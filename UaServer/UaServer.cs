using Opc.Ua;
using Opc.Ua.Configuration;
using System.Threading.Channels;
using Common;
using System;
using System.Threading.Tasks;

namespace UaServer
{
    public class UaServer
    {
        private readonly ApplicationInstance _application;
        private readonly NeuServer _server;
        private readonly string _user;
        private readonly string _password;
        private readonly ValueWrite _write;

        public Channel<Msg>? DataChannel { get; }
        public bool Running { get; private set; }

        private readonly Task _task;

        public UaServer(string uri, string user, string password, ValueWrite write)
        {
            _user = user;
            _password = password;
            _write = write;

            Running = false;
            DataChannel = Channel.CreateUnbounded<Msg>();

            var tokenPolicies = new List<UserTokenPolicy>
            {
                new UserTokenPolicy(UserTokenType.UserName),
                new UserTokenPolicy(UserTokenType.Certificate)
            };

            var serverConfiguration = new ServerConfiguration()
            {
                BaseAddresses = { uri },
                MinRequestThreadCount = 5,
                MaxRequestThreadCount = 100,
                MaxQueuedRequestCount = 200,
                UserTokenPolicies = new UserTokenPolicyCollection(tokenPolicies),
            };

            var securityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault",
                    SubjectName = Utils.Format(
                        @"CN={0}, DC={1}",
                        "neuopc",
                        System.Net.Dns.GetHostName()
                    )
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications"
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath =
                        @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates"
                },
                AutoAcceptUntrustedCertificates = true,
                AddAppCertToTrustedStore = true
            };

            var config = new ApplicationConfiguration
            {
                ApplicationName = "neuopc",
                ApplicationUri = Utils.Format(@"urn:{0}:neuopc", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = serverConfiguration,
                SecurityConfiguration = securityConfiguration,
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration()
            };

            config.Validate(ApplicationType.Server).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) =>
                {
                    e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted);
                };
            }

            _application = new ApplicationInstance
            {
                ApplicationName = "neuopc",
                ApplicationType = ApplicationType.Server,
                ApplicationConfiguration = config
            };

            _server = new NeuServer(_write);

            _task = new Task(async () =>
            {
                while (await DataChannel.Reader.WaitToReadAsync())
                {
                    if (DataChannel.Reader.TryRead(out var msg))
                    {
                        if (null != msg && null != msg.Items)
                        {
                            _server.UpdateNodes(msg.Items);
                        }
                    }
                }
            });
            _task.Start();
        }

        public void Start()
        {
            if (Running)
            {
                return;
            }

            bool certOk = _application.CheckApplicationInstanceCertificate(false, 0).Result;
            if (!certOk)
            {
                System.Diagnostics.Debug.WriteLine("check application instacnce cert failed");
            }

            try
            {
                _server.SetUser(_user, _password);
                _application.Start(_server).Wait();
                Running = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Stop()
        {
            if (Running)
            {
                try
                {
                    _application.Stop();
                }
                catch (Exception)
                {
                    throw;
                }

                Running = false;
            }

            System.Diagnostics.Debug.WriteLine("ua server stoped");
        }
    }
}

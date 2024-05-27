using Opc.Ua;
using Opc.Ua.Server;
using System.Security.Cryptography.X509Certificates;
using neulib;

namespace neuserver
{
    internal class NeuServer : StandardServer
    {
        private NeuNodeManager? _nodeManager;
        private readonly ValueWrite _write;
        private ICertificateValidator? _certificateValidator;
        private string? _user;
        private string? _password;

        public NeuServer(ValueWrite write)
        {
            _write = write;
        }

        public void UpdateNodes(List<Item> list)
        {
            _nodeManager?.UpdateNodes(list);
        }

        public void SetUser(string user, string password)
        {
            _user = user;
            _password = password;
        }

        protected override MasterNodeManager CreateMasterNodeManager(
            IServerInternal server,
            ApplicationConfiguration configuration
        )
        {
            Utils.Trace("Creating the Node Managers.");
            var nodeManagers = new List<INodeManager>();
            _nodeManager = new NeuNodeManager(server, configuration, _write);
            nodeManagers.Add(_nodeManager);
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            Utils.Trace("The server is starting.");

            base.OnServerStarting(configuration);
            CreateUserIdentityValidators(configuration);
        }

        private void CreateUserIdentityValidators(ApplicationConfiguration configuration)
        {
            for (int ii = 0; ii < configuration.ServerConfiguration.UserTokenPolicies.Count; ii++)
            {
                UserTokenPolicy policy = configuration.ServerConfiguration.UserTokenPolicies[ii];
                if (policy.TokenType == UserTokenType.Certificate)
                {
                    if (
                        configuration.SecurityConfiguration.TrustedUserCertificates != null
                        && configuration.SecurityConfiguration.UserIssuerCertificates != null
                    )
                    {
                        CertificateValidator certificateValidator = new CertificateValidator();
                        certificateValidator.Update(configuration.SecurityConfiguration).Wait();
                        certificateValidator.Update(
                            configuration.SecurityConfiguration.UserIssuerCertificates,
                            configuration.SecurityConfiguration.TrustedUserCertificates,
                            configuration.SecurityConfiguration.RejectedCertificateStore
                        );
                        _certificateValidator = certificateValidator.GetChannelValidator();
                    }
                }
            }
        }

        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);
            server.SessionManager.ImpersonateUser += SessionManagerImpersonateUser;
        }

        private void SessionManagerImpersonateUser(Session session, ImpersonateEventArgs args)
        {
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                VerifyPassword(userNameToken.UserName, userNameToken.DecryptedPassword);
                args.Identity = new UserIdentity(userNameToken);
                Utils.Trace("UserName Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }

            if (args.NewIdentity is X509IdentityToken x509Token)
            {
                VerifyCertificate(x509Token.Certificate);
                args.Identity = new UserIdentity(x509Token);
                Utils.Trace("X509 Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }
        }

        private void VerifyPassword(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw ServiceResultException.Create(
                    StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted."
                );
            }

            if (string.IsNullOrEmpty(password))
            {
                throw ServiceResultException.Create(
                    StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted."
                );
            }

            if (!(userName == _user && password == _password))
            {
                TranslationInfo info = new TranslationInfo(
                    "InvalidPassword",
                    "en-US",
                    "Specified password is not valid for user '{0}'.",
                    userName
                );

                throw new ServiceResultException(
                    new ServiceResult(
                        StatusCodes.BadIdentityTokenRejected,
                        "InvalidPassword",
                        LoadServerProperties().ProductUri,
                        new LocalizedText(info)
                    )
                );
            }
        }

        private void VerifyCertificate(X509Certificate2 certificate)
        {
            try
            {
                if (_certificateValidator != null)
                {
                    _certificateValidator.Validate(certificate);
                }
                else
                {
                    CertificateValidator.Validate(certificate);
                }
            }
            catch (Exception e)
            {
                TranslationInfo info;
                StatusCode result = StatusCodes.BadIdentityTokenRejected;
                if (
                    e is ServiceResultException se
                    && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed
                )
                {
                    info = new TranslationInfo(
                        "InvalidCertificate",
                        "en-US",
                        "'{0}' is an invalid user certificate.",
                        certificate.Subject
                    );

                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {
                    info = new TranslationInfo(
                        "UntrustedCertificate",
                        "en-US",
                        "'{0}' is not a trusted user certificate.",
                        certificate.Subject
                    );
                }

                throw new ServiceResultException(
                    new ServiceResult(
                        result,
                        info.Key,
                        "http://opcfoundation.org/UA/Sample/",
                        new LocalizedText(info)
                    )
                );
            }
        }
    }
}

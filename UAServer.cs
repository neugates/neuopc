using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using System.Threading.Channels;

namespace neuopc
{
    public delegate bool ValueWrite(Item item);

    public class NodeManager : CustomNodeManager2
    {
        private IList<IReference> _references;
        private FolderState folder;
        private List<BaseDataVariableState> variables;
        private ValueWrite write;

        public NodeManager(IServerInternal server, ApplicationConfiguration configuration, ValueWrite write)
            : base(server, configuration, "http://opcfoundation.org/Quickstarts/ReferenceApplications")
        {
            this.write = write;
            variables = new List<BaseDataVariableState>();
        }

        public void ResetNodes(List<Item> list)
        {
            lock (Lock)
            {
                foreach (var variable in variables)
                {
                    variable.StatusCode = StatusCodes.BadNotFound;
                    folder.RemoveChild(variable);
                }

                variables.Clear();

                foreach (var item in list)
                {
                    var variable = new BaseDataVariableState(folder)
                    {
                        SymbolicName = item.Name,
                        ReferenceTypeId = ReferenceTypes.Organizes,
                        TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                        NodeId = new NodeId(item.Name, NamespaceIndex),
                        BrowseName = new QualifiedName(item.Name, NamespaceIndex),
                        DisplayName = new LocalizedText("en", item.Name),
                        WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                        UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                        DataType = DataTypeIds.Double,
                        ValueRank = ValueRanks.Scalar,
                        AccessLevel = AccessLevels.CurrentReadOrWrite,
                        UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                        Historizing = false,
                        StatusCode = StatusCodes.Good,
                        Timestamp = DateTime.Now,
                        OnWriteValue = OnWriteDataValue,
                        OnReadValue = OnReadDataValue
                    };

                    SetVariable(variable, item);

                    if (variable.ValueRank == ValueRanks.OneDimension)
                    {
                        variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
                    }
                    else if (variable.ValueRank == ValueRanks.TwoDimensions)
                    {
                        variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
                    }

                    if (null != folder)
                    {
                        folder.AddChild(variable);
                    }

                    variables.Add(variable);
                }


                AddPredefinedNode(SystemContext, folder);
            }
        }

        public void UpdateNodes(List<Item> list)
        {
            try
            {
                lock (Lock)
                {
                    foreach (var item in list)
                    {
                        int index = item.ClientHandle;
                        var variable = variables[index];
                        SetVariable(variable, item);
                        variable.ClearChangeMasks(SystemContext, false);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Warning($"set variable exception, error:{exception.Message}");
            }
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out _references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = _references = new List<IReference>();
                }

                string folderName = "NeuOPC";
                folder = new FolderState(null)
                {
                    SymbolicName = folderName,
                    ReferenceTypeId = ReferenceTypes.Organizes,
                    TypeDefinitionId = ObjectTypeIds.FolderType,
                    NodeId = new NodeId(folderName, NamespaceIndex),
                    BrowseName = new QualifiedName(folderName, NamespaceIndex),
                    DisplayName = new LocalizedText("en", folderName),
                    WriteMask = AttributeWriteMask.None,
                    UserWriteMask = AttributeWriteMask.None,
                    EventNotifier = EventNotifiers.None
                };

                folder.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                _references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, folder.NodeId));
                folder.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(folder);
                AddPredefinedNode(SystemContext, folder);
            }
        }

        private void SetVariable(BaseDataVariableState variable, Item item)
        {
            if (1001 == item.Error)
            {
                variable.StatusCode = StatusCodes.BadNotReadable;
            }

            variable.Timestamp = item.Timestamp;
            switch (item.Type)
            {
                case DaType.Int8:
                    variable.DataType = DataTypeIds.SByte;
                    variable.Value = (sbyte)(item.Value ?? 0);
                    break;
                case DaType.Int16:
                    variable.DataType = DataTypeIds.Int16;
                    variable.Value = (short)(item.Value ?? 0);
                    break;
                case DaType.Int32:
                    variable.DataType = DataTypeIds.Int32;
                    variable.Value = (int)(item.Value ?? 0);
                    break;
                case DaType.Int32B:
                    variable.DataType = DataTypeIds.Int32;
                    variable.Value = (int)(item.Value ?? 0);
                    break;
                case DaType.Int64:
                    variable.DataType = DataTypeIds.Int64;
                    variable.Value = (long)(item.Value ?? 0);
                    break;
                case DaType.Float:
                    variable.DataType = DataTypeIds.Float;
                    variable.Value = (float)(item.Value ?? 0);
                    break;
                case DaType.Double:
                    variable.DataType = DataTypeIds.Double;
                    variable.Value = (double)(item.Value ?? 0);
                    break;
                case DaType.UInt8:
                    variable.DataType = DataTypeIds.Byte;
                    variable.Value = (byte)(item.Value ?? 0);
                    break;
                case DaType.UInt16:
                    variable.DataType = DataTypeIds.UInt16;
                    variable.Value = (ushort)(item.Value ?? 0);
                    break;
                case DaType.UInt32:
                    variable.DataType = DataTypeIds.UInt32;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case DaType.UInt32B:
                    variable.DataType = DataTypeIds.UInt32;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case DaType.UInt64:
                    variable.DataType = DataTypeIds.UInt64;
                    variable.Value = (ulong)(item.Value ?? 0);
                    break;
                case DaType.Date:
                    {
                        variable.DataType = DataTypeIds.DateTime;
                        try
                        {
                            variable.Value = (DateTime)(item.Value ?? DateTime.Now);
                        }
                        catch
                        {
                            variable.StatusCode = StatusCodes.BadNotReadable;
                        }

                        break;
                    }
                case DaType.String:
                    variable.DataType = DataTypeIds.String;
                    variable.Value = item.Value as string;
                    break;
                case DaType.Bool:
                    {
                        variable.DataType = DataTypeIds.Boolean;
                        try
                        {
                            variable.Value = (bool)(item.Value ?? false);
                        }
                        catch
                        {
                            variable.StatusCode = StatusCodes.BadNotReadable;
                        }

                        break;
                    }
                default:
                    {
                        variable.DataType = DataTypeIds.BaseDataType;
                        variable.StatusCode = StatusCodes.BadNotSupported;
                        break;
                    }
            }
        }

        private ServiceResult OnReadDataValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding,
            ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            return ServiceResult.Good;
        }

        private ServiceResult OnWriteDataValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding,
            ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            var variable = node as BaseDataVariableState;
            var item = new Item();
            try
            {
                TypeInfo typeInfo = TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || typeInfo == TypeInfo.Unknown)
                {
                    return StatusCodes.BadTypeMismatch;
                }

                item.Name = variable.SymbolicName;
                item.Value = value;
                switch (typeInfo.BuiltInType)
                {
                    case BuiltInType.SByte:
                        item.Type = DaType.Int8;
                        break;
                    case BuiltInType.Int16:
                        item.Type = DaType.Int16;
                        break;
                    case BuiltInType.Int32:
                        item.Type = DaType.Int32;
                        break;
                    case BuiltInType.Int64:
                        item.Type = DaType.Int64;
                        break;
                    case BuiltInType.Float:
                        item.Type = DaType.Float;
                        break;
                    case BuiltInType.Double:
                        item.Type = DaType.Double;
                        break;
                    case BuiltInType.Byte:
                        item.Type = DaType.UInt8;
                        break;
                    case BuiltInType.UInt16:
                        item.Type = DaType.UInt16;
                        break;
                    case BuiltInType.UInt32:
                        item.Type = DaType.UInt32;
                        break;
                    case BuiltInType.UInt64:
                        item.Type = DaType.UInt64;
                        break;
                    case BuiltInType.DateTime:
                        item.Type = DaType.Date;
                        break;
                    case BuiltInType.String:
                        item.Type = DaType.String;
                        break;
                    case BuiltInType.Boolean:
                        item.Type = DaType.Bool;
                        break;
                    default:
                        break;
                }

                var result = write?.Invoke(item);
                if (result is true)
                {
                    return ServiceResult.Good;
                }

                return StatusCodes.BadNotWritable;
            }
            catch (Exception)
            {
                return StatusCodes.BadTypeMismatch;
            }
        }
    }

    public class Server : StandardServer
    {
        private NodeManager nodeManager;
        private ValueWrite write;
        private ICertificateValidator m_certificateValidator;
        private string user;
        private string password;

        public Server(ValueWrite write)
        {
            this.write = write;
        }

        public NodeManager GetNodeManager()
        {
            return nodeManager;
        }

        public void ResetNodes(List<Item> list)
        {
            nodeManager?.ResetNodes(list);
        }

        public void UpdateNodes(List<Item> list)
        {
            nodeManager?.UpdateNodes(list);
        }

        public void SetUser(string user, string password)
        {
            this.user = user;
            this.password = password;
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");
            var nodeManagers = new List<INodeManager>();
            nodeManager = new NodeManager(server, configuration, write);
            nodeManagers.Add(nodeManager);
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
                    if (configuration.SecurityConfiguration.TrustedUserCertificates != null &&
                        configuration.SecurityConfiguration.UserIssuerCertificates != null)
                    {
                        CertificateValidator certificateValidator = new CertificateValidator();
                        certificateValidator.Update(configuration.SecurityConfiguration).Wait();
                        certificateValidator.Update(configuration.SecurityConfiguration.UserIssuerCertificates,
                            configuration.SecurityConfiguration.TrustedUserCertificates,
                            configuration.SecurityConfiguration.RejectedCertificateStore);
                        m_certificateValidator = certificateValidator.GetChannelValidator();
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
            UserNameIdentityToken userNameToken = args.NewIdentity as UserNameIdentityToken;
            if (userNameToken != null)
            {
                VerifyPassword(userNameToken.UserName, userNameToken.DecryptedPassword);
                args.Identity = new UserIdentity(userNameToken);
                Utils.Trace("UserName Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }

            X509IdentityToken x509Token = args.NewIdentity as X509IdentityToken;
            if (x509Token != null)
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
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted.");
            }


            if (!(userName == this.user && password == this.password))
            {
                TranslationInfo info = new TranslationInfo(
                    "InvalidPassword",
                    "en-US",
                    "Specified password is not valid for user '{0}'.",
                    userName);

                throw new ServiceResultException(new ServiceResult(
                    StatusCodes.BadIdentityTokenRejected,
                    "InvalidPassword",
                    LoadServerProperties().ProductUri,
                    new LocalizedText(info)));
            }
        }

        private void VerifyCertificate(X509Certificate2 certificate)
        {
            try
            {
                if (m_certificateValidator != null)
                {
                    m_certificateValidator.Validate(certificate);
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
                ServiceResultException se = e as ServiceResultException;
                if (se != null && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    info = new TranslationInfo(
                        "InvalidCertificate",
                        "en-US",
                        "'{0}' is an invalid user certificate.",
                        certificate.Subject);

                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {
                    info = new TranslationInfo(
                        "UntrustedCertificate",
                        "en-US",
                        "'{0}' is not a trusted user certificate.",
                        certificate.Subject);
                }

                throw new ServiceResultException(new ServiceResult(
                    result,
                    info.Key,
                    "http://opcfoundation.org/UA/Sample/",
                    new LocalizedText(info)));
            }
        }
    }

    public class UAServer
    {
        private ApplicationInstance application;
        private bool running;
        private Server server;
        public ValueWrite Write;

        public Channel<DaMsg> channel;
        private Task task;
        List<Item> list;

        public UAServer()
        {
            running = false;
            channel = Channel.CreateUnbounded<DaMsg>();
            task = new Task(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var msg))
                    {
                        if (MsgType.List == msg.Type)
                        {
                            ResetNodes(msg.Items);
                        }
                        else if (MsgType.Data == msg.Type)
                        {
                            UpdateNodes(msg.Items);
                        }
                    }
                }
            });
            task.Start();
        }

        public void Start(string port, string user, string password)
        {
            if (running)
            {
                return;
            }

            string uri = $"opc.tcp://localhost:{port}/";
            try
            {
                var tokenPolicies = new List<UserTokenPolicy>();
                tokenPolicies.Add(new UserTokenPolicy(UserTokenType.UserName));
                tokenPolicies.Add(new UserTokenPolicy(UserTokenType.Certificate));
                var config = new ApplicationConfiguration()
                {
                    ApplicationName = "neuopc",
                    ApplicationUri = Utils.Format(@"urn:{0}:neuopc", System.Net.Dns.GetHostName()),
                    ApplicationType = ApplicationType.Server,
                    ServerConfiguration = new ServerConfiguration()
                    {
                        BaseAddresses = { uri },
                        MinRequestThreadCount = 5,
                        MaxRequestThreadCount = 100,
                        MaxQueuedRequestCount = 200,
                        UserTokenPolicies = new UserTokenPolicyCollection(tokenPolicies),
                    },

                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "AxiuOpcua", System.Net.Dns.GetHostName()) },
                        TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                        TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                        RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                        AutoAcceptUntrustedCertificates = true,
                        AddAppCertToTrustedStore = true
                    },

                    TransportConfigurations = new TransportConfigurationCollection(),
                    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                    TraceConfiguration = new TraceConfiguration()
                };

                config.Validate(ApplicationType.Server).GetAwaiter().GetResult();
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
                }

                application = new ApplicationInstance
                {
                    ApplicationName = "neuopc",
                    ApplicationType = ApplicationType.Server,
                    ApplicationConfiguration = config
                };

                bool certOk = application.CheckApplicationInstanceCertificate(false, 0).Result;
                if (!certOk)
                {
                    // TODO: log
                }

                server = new Server(Write);
                application.Start(server).Wait();
                running = true;
                server.SetUser(user, password);
            }
            catch (Exception)
            {
            }

            if (null != this.list)
            {
                server?.ResetNodes(this.list);
            }
        }

        public bool IsRunning()
        {
            return running;
        }

        public void ResetNodes(List<Item> list)
        {
            this.list = list;
            server?.ResetNodes(list);
        }

        public void UpdateNodes(List<Item> list)
        {
            server?.UpdateNodes(list);
        }

        public void Stop()
        {
            channel.Writer.Complete();
            task.Wait();
            running = false;
            //application?.Stop();
        }
    }
}

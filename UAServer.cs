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

namespace neuopc
{
    public class NodeManager : CustomNodeManager2
    {
        private IList<IReference> _references;
        private List<Item> items;
        private List<BaseDataVariableState> variables;

        public NodeManager(IServerInternal server, ApplicationConfiguration configuration, List<Item> items)
            : base(server, configuration, "http://opcfoundation.org/Quickstarts/ReferenceApplications")
        {
            this.items = items;
            variables = new List<BaseDataVariableState>();
        }

        public void SetItems(List<Item> list)
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
            catch
            {
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
                var folder = new FolderState(null)
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

                foreach (var item in items)
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

        private void SetVariable(BaseDataVariableState variable, Item item)
        {
            variable.Timestamp = item.Timestamp;
            switch (item.Type)
            {
                case DAType.Int8:
                    variable.DataType = DataTypeIds.SByte;
                    variable.Value = (sbyte)(item.Value ?? 0);
                    break;
                case DAType.Int16:
                    variable.DataType = DataTypeIds.Int16;
                    variable.Value = (short)(item.Value ?? 0);
                    break;
                case DAType.Int32:
                    variable.DataType = DataTypeIds.Int32;
                    variable.Value = (int)(item.Value ?? 0);
                    break;
                case DAType.Int32B:
                    variable.DataType = DataTypeIds.Int32;
                    variable.Value = (int)(item.Value ?? 0);
                    break;
                case DAType.Int64:
                    variable.DataType = DataTypeIds.Int64;
                    variable.Value = (long)(item.Value ?? 0);
                    break;
                case DAType.Float:
                    variable.DataType = DataTypeIds.Float;
                    variable.Value = (float)(item.Value ?? 0);
                    break;
                case DAType.Double:
                    variable.DataType = DataTypeIds.Double;
                    variable.Value = (double)(item.Value ?? 0);
                    break;
                case DAType.UInt8:
                    variable.DataType = DataTypeIds.Byte;
                    variable.Value = (byte)(item.Value ?? 0);
                    break;
                case DAType.UInt16:
                    variable.DataType = DataTypeIds.UInt16;
                    variable.Value = (ushort)(item.Value ?? 0);
                    break;
                case DAType.UInt32:
                    variable.DataType = DataTypeIds.UInt32;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case DAType.UInt32B:
                    variable.DataType = DataTypeIds.UInt32;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case DAType.UInt64:
                    variable.DataType = DataTypeIds.UInt64;
                    variable.Value = (ulong)(item.Value ?? 0);
                    break;
                case DAType.Date:
                    variable.DataType = DataTypeIds.DateTime;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case DAType.String:
                    variable.DataType = DataTypeIds.String;
                    variable.Value = item.Value as string;
                    break;
                case DAType.Bool:
                    variable.DataType = DataTypeIds.Boolean;
                    variable.Value = (bool)(item.Value ?? false);
                    break;
                default:
                    variable.DataType = DataTypeIds.String;
                    variable.Value = "no supported data";
                    break;
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
            BaseDataVariableState variable = node as BaseDataVariableState;
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
                if (typeInfo.BuiltInType == BuiltInType.Double)
                {
                    double number = Convert.ToDouble(value);
                    value = TypeInfo.Cast(number, typeInfo.BuiltInType);
                }
                return ServiceResult.Good;
            }
            catch (Exception)
            {
                return StatusCodes.BadTypeMismatch;
            }
        }
    }

    public class Server : StandardServer
    {
        private List<Item> items;
        private NodeManager nodeManager;

        public Server(List<Item> items)
        {
            this.items = items;
        }

        public void SetItems(List<Item> list)
        {
            nodeManager.SetItems(list);
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");
            var nodeManagers = new List<INodeManager>();
            nodeManager = new NodeManager(server, configuration, items);
            nodeManagers.Add(nodeManager);
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }
    }

    public class UAServer
    {
        private ApplicationInstance application;
        private List<Item> items;
        private Server server;

        public void Start(string port, List<Item> items)
        {
            this.items = items;
            string uri = $"opc.tcp://localhost:{port}/";
            try
            {
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

                //application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();
                bool certOk = application.CheckApplicationInstanceCertificate(false, 0).Result;
                if (!certOk)
                {
                    //Console.WriteLine("证书验证失败!");
                }

                var dis = new DiscoveryServerBase();
                server = new Server(items);
                application.Start(server).Wait();
            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateNodes(List<Item> list)
        {
            server.SetItems(list);
        }

        public void Stop()
        {
            application?.Stop();
        }
    }
}

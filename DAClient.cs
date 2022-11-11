using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using OPCAutomation;
using System.Net;
using System.Linq;
using System.Threading;
using Serilog;

namespace neuopc
{
    public delegate void ValueUpdate(List<Item> items);

    public class Node
    {
        public string Name { get; set; }
        public DAType Type { get; set; }
        public OPCItem Item { get; set; }
    }

    public enum DAType
    {
        Int8 = 16, // UA_TYPES_SBYTE/VT_I1
        Int16 = 2, // UA_TYPES_INT16/VT_I2
        Int32B = 22, // UA_TYPES_INT32/VT_INT 
        Int32 = 3, // UA_TYPES_INT32/VT_I4
        Int64 = 20, // UA_TYPES_INT64/VT_I8
        Float = 4, // UA_TYPES_FLOAT/VT_R4
        Double = 5, // UA_TYPES_DOUBLE/VT_R8
        UInt8 = 17, // UA_TYPES_BYTE/VT_UI1
        UInt16 = 18, // UA_TYPES_UINT16/VT_UI2
        UInt32B = 23, // UA_TYPES_UINT32/VT_UINT
        UInt32 = 19, // UA_TYPES_UINT32/VT_UI4
        UInt64 = 21, // UA_TYPES_UINT64/VT_UI8
        Date = 7, // UA_TYPES_DATETIME/VT_DATE
        String = 8, // UA_TYPES_STRING/VT_BSTR
        Bool = 11, // UA_TYPES_BOOLEAN/VT_BOOL
        Money = 6, // Money
    }

    public enum DARights
    {
        Read = 1,
        Write = 2,
        RW = 3,
    }

    public enum DAQuality
    {
        Good = 192,
        Bad = 0,
    }

    public class Item
    {
        public string Name { get; set; }
        public DAType Type { get; set; }
        public DARights Rights { get; set; }
        public int ClientHandle { get; set; }
        public int ServerHandle { get; set; }
        public dynamic Value { get; set; }
        public DAQuality Quality { get; set; }
        public DateTime Timestamp { get; set; }
        public int Error { get; set; }
        public string Status { get; set; }
    }

    public class DAClient
    {
        private OPCServer server;
        private OPCBrowser brower;
        private OPCGroups groups;
        private OPCGroup group;
        private List<Node> nodes;
        private Thread thread;
        private bool running;
        private string hostName;
        private string serverName;
        public ValueUpdate Update;
        private readonly int MaxRead= 50;

        public DAClient()
        {
        }

        public void Setup()
        {
            int count = 3;
            do
            {
                bool flag = true;
                try
                {
                    var testServer = new OPCServer();
                }
                catch (Exception)
                {
                    flag = false;
                }

                if (flag)
                {
                    break;
                }

                // TODO: regist com component
                count--;
            } while (0 < count);
        }

        public (bool ok, string msg) Connect(string host, string name)
        {
            hostName = host;
            serverName = name;

            if (IsConnected())
            {
                DisConnect();
            }

            try
            {
                server = new OPCServer();
                server.Connect(name, host);
            }
            catch (Exception ex)
            {
                server = null;
                return (false, ex.Message);
            }

            Log.Information($"server {host}/{name} connected");
            return (true, "success");
        }

        public bool IsConnected()
        {
            if (null == server)
            {
                return false;
            }

            return 0 == server.ServerState;
        }

        public void DisConnect()
        {
            try
            {
                server?.Disconnect();
            }
            catch (Exception)
            {
            }
        }

        public List<string> GetHosts()
        {
            var list = new List<string>();
            IPHostEntry ipHost = Dns.GetHostEntry("127.0.0.1");
            list.Add(ipHost.HostName);
            // TODO: enum all host
            return list;
        }

        public List<string> GetServers(string host)
        {
            var list = new List<string>();
            try
            {
                var opcServer = new OPCServer();
                object servers = opcServer.GetOPCServers(host);
                foreach (string s in (Array)servers)
                {
                    list.Add(s);
                }
            }
            catch (Exception exception)
            {
                Log.Warning($"get opc servers failed, error:{exception.Message}");
            }

            return list;
        }

        public List<Item> BuildGroup()
        {
            try
            {
                brower = server?.CreateBrowser();
                brower?.ShowBranches();
                brower?.ShowLeafs(true);
            }
            catch (Exception error)
            {
                Log.Error($"create browser failed, msg:{error.Message}");
                brower = null;
                return new List<Item>();
            }

            try
            {
                groups = server.OPCGroups;
                groups.DefaultGroupIsActive = true;
                group = groups.Add("all");
                group.IsActive = true;
            }
            catch (Exception error)
            {
                Log.Error($"add group failed, msg:{error.Message}");
                return new List<Item>();
            }

            nodes = new List<Node>();
            int index = 0;
            foreach (var item in brower)
            {
                var node = new Node()
                {
                    Name = item.ToString(),
                };

                try
                {
                    node.Item = group.OPCItems.AddItem(node.Name, index);
                    node.Type = (DAType)node.Item.CanonicalDataType;
                    nodes.Add(node);
                    index++;
                }
                catch (Exception exception)
                {
                    Log.Warning($"add item failed, name:{item}, error:{exception.Message}");
                }

                Log.Information($"add item secceed, name:{item}, type:{node.Type}");
            }

            var list = (from node in nodes
                        let name = node.Name
                        select new Item
                        {
                            Name = name,
                            Type = (DAType)node.Item.CanonicalDataType,
                            Rights = (DARights)node.Item.AccessRights,
                            ClientHandle = node.Item.ClientHandle,
                            ServerHandle = node.Item.ServerHandle,
                            Quality = 0,
                            Timestamp = DateTime.Now,
                            Status = string.Empty,
                        }).ToList();
            return list;
        }

        private void ReadThread()
        {
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupUpdateRate = 200;

            group.IsSubscribed = true;
            group.UpdateRate = 200;
            group.DataChange += GroupDataChange;

            while (running)
            {
                int count = null == nodes ? 0 : nodes.Count;
                int t1 = count / MaxRead;
                int t2 = (count % MaxRead) == 0 ? 0 : 1;
                int times = t1 + t2;
                for (int i = 0; i < times; i++)
                {
                    var tmpNode = from node in nodes.Skip(i * MaxRead).Take(MaxRead)
                                  select node;
                    var nodeList = tmpNode.ToList();

                    var tmp = from node in tmpNode
                              select node.Item.ServerHandle;
                    List<int> l = tmp.ToList();
                    l.Insert(0, 0);
                    Array hs = l.ToArray();
                    var items = new List<Item>();

                    try
                    {
                        short source = 1;
                        group.SyncRead(source, tmpNode.Count(), ref hs, out Array values, out Array errors, out dynamic qualities, out dynamic timestamps);
                        Array qs = (Array)qualities;
                        Array ts = (Array)timestamps;

                        for (int j = 0; j < nodeList.Count; j++)
                        {
                            var n = nodeList[j];
                            var item = new Item
                            {
                                Name = n.Name,
                                ClientHandle = n.Item.ClientHandle,
                                Type = n.Type,
                                Value = values.GetValue(j + 1),
                                Rights = (DARights)n.Item.AccessRights,
                                Quality = (DAQuality)Convert.ToInt32(qs.GetValue(j + 1)),
                                Error = Convert.ToInt32(errors.GetValue(j + 1)),
                                Timestamp = Convert.ToDateTime(ts.GetValue(j + 1)).ToLocalTime(),
                            };
                            items.Add(item);
                        }
                    }
                    catch (Exception)
                    {
                        //Connect(host_name, server_name);
                    }

                    Update?.Invoke(items);

                }

                Thread.Sleep(5000);
            }
        }

        private void GroupDataChange(int transactionID, int numItems, ref Array clientHandles, ref Array itemValues, ref Array qualities, ref Array timeStamps)
        {
            var items = new List<Item>();
            for (int i = 1; i <= numItems; i++)
            {
                int handle = Convert.ToInt32(clientHandles.GetValue(i));
                var node = nodes.FirstOrDefault(node => node.Item.ClientHandle == handle);
                if (null == node)
                {
                    continue;
                }

                var item = new Item
                {
                    Name = node.Name,
                    ClientHandle = handle,
                    Type = node.Type,
                    Value = itemValues.GetValue(i),
                    Rights = (DARights)node.Item.AccessRights,
                    Quality = (DAQuality)Convert.ToInt32(qualities.GetValue(i)),
                    Timestamp = Convert.ToDateTime(timeStamps.GetValue(i)).ToLocalTime(),
                };

                items.Add(item);
            }

            Update?.Invoke(items);
        }

        public void Read()
        {
            if (null == server) { return; }
            if (null == group) { return; }

            running = true;
            ThreadStart ts = new ThreadStart(ReadThread);
            thread = new Thread(ts);
            thread.Start();
        }

        public bool Write(Item item)
        {
            var writeNode = nodes?.FirstOrDefault(node => node.Name == item.Name);
            if (null != writeNode)
            {
                try
                {
                    writeNode.Item.Write(item.Value);
                }
                catch (Exception exception)
                {
                    Log.Error($"write node failed, name:{writeNode.Name}, value:{item.Value}, error:{exception.Message}");
                    return false;
                }

                Log.Information($"write node succeed, name:{writeNode.Name}, value:{item.Value}");
                return true;
            }
            else
            {
                Log.Error($"write node failed, name:{writeNode.Name}, value:{item.Value}");
                return false;
            }
        }

        public void Start() { }

        public void Stop()
        {
            running = false;
            thread?.Join();
            DisConnect();
        }
    }
}

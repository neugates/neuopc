using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using OPCAutomation;
using System.Net;
//using System.Timers;
using System.Linq;
using System.Threading;

namespace neuopc
{
    public delegate void ValueUpdate(List<Item> items);

    public class Node
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public OPCItem Item { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int Rights { get; set; } // 1-read only, 2-write only, 3-read & write
        public int ClientHandle { get; set; }
        public int ServerHandle { get; set; }
        public dynamic Value { get; set; }
        public int Quality { get; set; }
        public string Timestamp { get; set; }
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

        private readonly int MAX_READ = 50;

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
                catch (Exception ex)
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
            server?.Disconnect();
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
                var serverTemp = new OPCServer();
                object servers = serverTemp.GetOPCServers(host);
                foreach (string s in (Array)servers)
                {
                    list.Add(s);
                }
            }
            catch (Exception ex)
            {
            }

            return list;
        }

        public List<Item> BuildGroup()
        {
            try
            {
                brower = server?.CreateBrowser();
            }
            catch (Exception ex)
            {
                // TODO:Log
            }

            if (null == brower)
            {
                return new List<Item>();
            }

            brower.ShowBranches();
            brower.ShowLeafs(true);

            groups = server.OPCGroups;
            groups.DefaultGroupIsActive = true;
            //groups.DefaultGroupDeadband = 0;
            //groups.DefaultGroupUpdateRate = 200;

            group = groups.Add("all");
            group.IsActive = true;
            //group.IsSubscribed = true;
            //group.UpdateRate = 200;
            //group.DataChange += GroupDataChange;

            nodes = new List<Node>();
            int index = 0;
            foreach (var item in brower)
            {
                var node = new Node()
                {
                    Name = item.ToString(),
                };

                node.Item = group.OPCItems.AddItem(node.Name, index);

                System.Diagnostics.Debug.WriteLine($"------>{node.Item.Parent.Name}");
                nodes.Add(node);
                index++;
            }

            var temp = from node in nodes
                       let name = node.Name
                       select new Item
                       {
                           Name = name,
                           Type = node.Item.CanonicalDataType,
                           Rights = node.Item.AccessRights,
                           ClientHandle = node.Item.ClientHandle,
                           ServerHandle = node.Item.ServerHandle,
                           Value = string.Empty,
                           Quality = 0,
                           Timestamp = string.Empty,
                           Status = string.Empty,
                       };
            List<Item> list = temp.ToList();
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
                int t1 = nodes.Count / MAX_READ;
                int t2 = (nodes.Count % MAX_READ) == 0 ? 0 : 1;
                int times = t1 + t2;
                for (int i = 0; i < times; i++)
                {
                    var tmpNode = from node in nodes.Skip(i * MAX_READ).Take(MAX_READ)
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
                                Value = values.GetValue(j + 1),
                                Rights = n.Item.AccessRights,
                                Quality = Convert.ToInt32(qs.GetValue(j + 1)),
                                Error = Convert.ToInt32(errors.GetValue(j + 1)),
                                Timestamp = Convert.ToString(ts.GetValue(j + 1)),
                            };
                            items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Connect(host_name, server_name);
                    }

                    Update?.Invoke(items);

                }
                Thread.Sleep(5000);
            }
        }

        private void GroupDataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            var items = new List<Item>();
            for (int i = 1; i <= NumItems; i++)
            {
                int clientHandle = Convert.ToInt32(ClientHandles.GetValue(i));
                var node = from n in nodes
                           where n.Item.ClientHandle == clientHandle
                           select n;

                var item = new Item
                {
                    Name = node.First().Name,
                    ClientHandle = clientHandle,
                    Value = ItemValues.GetValue(i),
                    Rights = node.First().Item.AccessRights,
                    Quality = Convert.ToInt32(Qualities.GetValue(i)),
                    Timestamp = Convert.ToString(TimeStamps.GetValue(i)),
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

        public void Write() { }

        public void Start() { }

        public void Stop()
        {
            running = false;
            thread?.Join();
            DisConnect();
        }
    }
}

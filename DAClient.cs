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
        private string host_name;
        private string server_name;
        public ValueUpdate update;

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
            host_name = host;
            server_name = name;

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

            nodes = new List<Node>();
            int index = 0;
            foreach (var item in brower)
            {
                var node = new Node()
                {
                    Name = item.ToString(),
                };

                node.Item = group.OPCItems.AddItem(node.Name, index);
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
            while (running)
            {
                int t1 = nodes.Count / MAX_READ;
                int t2 = (nodes.Count % MAX_READ) == 0 ? 0 : 1;
                int times = t1 + t2;
                for (int i = 0; i < times; i++)
                {
                    var tmp_node = from node in nodes.Skip(i * MAX_READ)
                                   select node;

                    var tmp = from node in tmp_node
                              select node.Item.ServerHandle;
                    List<int> l = tmp.ToList();
                    l.Insert(0, 0);
                    Array hs = l.ToArray();

                    var items = new List<Item>();

                    try
                    {
                        short source = (short)(i * MAX_READ);
                        group.SyncRead(source, l.Count, ref hs, out Array values, out Array errors, out dynamic qualities, out dynamic timestamps);
                        Array qs = (Array)qualities;
                        Array ts = (Array)timestamps;
                        foreach (var n in tmp_node)
                        {
                            var item = new Item
                            {
                                Name = n.Name,
                                ClientHandle = n.Item.ClientHandle,
                                Value = values.GetValue(i + 1),
                                Rights = n.Item.AccessRights,
                                Quality = Convert.ToInt32(qs.GetValue(i + 1)),
                                Error = Convert.ToInt32(errors.GetValue(i + 1)),
                                Timestamp = Convert.ToString(ts.GetValue(i + 1)),
                            };
                            items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Connect(host_name, server_name);
                    }

                    update?.Invoke(items);

                }

                //var temp = from node in nodes
                //           select node.Item.ServerHandle;
                //List<int> list = temp.ToList();
                //list.Insert(0, 0);
                //Array handles = list.ToArray();
                //var items = new List<Item>();

                //try
                //{
                //    short source = 1;
                //    group.SyncRead(source, nodes.Count, ref handles, out Array values, out Array errors, out dynamic qualities, out dynamic timestamps);
                //    Array qs = (Array)qualities;
                //    Array ts = (Array)timestamps;
                //    for (int i = 0; i < nodes.Count; i++)
                //    {
                //        var node = nodes[i];
                //        var item = new Item
                //        {
                //            Name = node.Name,
                //            ClientHandle = node.Item.ClientHandle,
                //            Value = values.GetValue(i + 1),
                //            Rights = node.Item.AccessRights,
                //            Quality = Convert.ToInt32(qs.GetValue(i + 1)),
                //            Error = Convert.ToInt32(errors.GetValue(i + 1)),
                //            Timestamp = Convert.ToString(ts.GetValue(i + 1)),
                //        };
                //        items.Add(item);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Connect(host_name, server_name);
                //}

                Thread.Sleep(100);
                //update?.Invoke(items);
            }
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
            thread.Join();
        }
    }
}

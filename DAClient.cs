using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using OPCAutomation;
using System.Net;
using System.Timers;

namespace neuopc
{
    public class Node
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public OPCItem Item { get; set; }
    }

    public class DAClient
    {
        private OPCServer server;
        private OPCBrowser brower;
        private OPCGroups groups;
        private OPCGroup group;
        private List<Node> nodes;
        private Timer timer;

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

        public (bool ok, string msg) Conenct(string host, string name)
        {
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

        public void BuildGroup()
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
                return;
            }

            brower.ShowBranches();
            brower.ShowLeafs(true);

            groups = server.OPCGroups;
            groups.DefaultGroupIsActive = true;
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupUpdateRate = 200;

            group = groups.Add("all");
            group.IsActive = true;
            group.IsSubscribed = true;
            group.UpdateRate = 200;

            nodes = new List<Node>();
            int index = 0;
            foreach (var item in brower)
            {
                var node = new Node()
                {
                    Name = item.ToString(),
                    Id = index,
                };

                node.Item = group.OPCItems.AddItem(node.Name, node.Id);
                nodes.Add(node);
                index++;
            }
        }

        private void GroupAsyncReadComplete(int TransactionID, int NumItems, ref System.Array ClientHandles, ref System.Array ItemValues, ref System.Array Qualities, ref System.Array TimeStamps, ref System.Array Errors)
        {
            System.Diagnostics.Debug.WriteLine("====================GroupAsyncReadComplete");
            for (int i = 1; i <= NumItems; i++)
            {
                //Console.WriteLine("Tran：{0}   ClientHandles：{1}   Error：{2}", TransactionID.ToString(), ClientHandles.GetValue(i).ToString(), Errors.GetValue(i).ToString());
                //System.Diagnostics.Debug.WriteLine("Vaule：{0}", Convert.ToString(ItemValues.GetValue(i)));
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("====================Timer");
        }

        public void Read()
        {
            if (null == server) { return; }
            if (null == group) { return; }

            group.AsyncReadComplete += GroupAsyncReadComplete;
            timer = new Timer();
            timer.Elapsed += OnTimer;
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Enabled = false;
            timer.Start();
        }

        public void Write() { }

        public void Start() { }

        public void Stop()
        {
            timer.Stop();
        }

    }
}

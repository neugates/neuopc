using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Opc;
using OPCAutomation;

namespace neuopc
{
    public class Node
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class DAClient
    {
        private OPCServer server = null;
        private OPCBrowser brower = null;
        private OPCGroups groups = null;
        private OPCGroup group = null;
        private List<Node> nodes;

        public DAClient()
        {
        }

        public (bool ok, string msg) Conenct(string host, string name)
        {
            try
            {
                server = new OPCServer();
                server.Connect(host, name);
            }
            catch (Exception ex)
            {
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
            brower = server?.CreateBrowser();
            if (null == brower)
            {
                return;
            }

            brower.ShowBranches();
            brower.ShowLeafs(true);
            nodes.Clear();
            int index = 0;
            foreach (var item in brower)
            {
                var node = new Node()
                {
                    Name = item.ToString(),
                    Id = index,
                };
                nodes.Add(node);
                index++;
            }

            groups = server.OPCGroups;
            groups.DefaultGroupIsActive = true;
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupUpdateRate = 200;

            group = groups.Add("all");
            group.IsActive = true;
            group.IsSubscribed = true;
            group.UpdateRate = 200;
            //group.AsyncReadComplete += Group_AsyncReadComplete;

            foreach (var node in nodes)
            {
                group.OPCItems.AddItem(node.Name, node.Id);
            }
        }

    }
}

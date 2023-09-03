using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using neuclient;
using Opc.Ua.Server;
using Serilog;

namespace neuopc
{
    internal class Client
    {
        private static DaClient _client = null;
        private static bool _clientRunning = false;
        private static Thread _clientThread = null;
        private static Dictionary<string, Node> _nodeMap = null;

        private static bool UpdateNodeMap()
        {
            bool upgrade = false;
            var nodes = DaBrowse.AllNode(_client.Server);
            foreach (var node in nodes)
            {
                if (!_nodeMap.ContainsKey(node.Name))
                {
                    upgrade = true;
                    _nodeMap.Add(node.Name, node);
                    Log.Information($"add item:{node.Name}");
                }
            }

            return upgrade;
        }

        private static void ClientThread()
        {
            int count = 0;
            while (_clientRunning)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "connect to server failed");
                    Thread.Sleep(3000);
                    continue;
                }

                Thread.Sleep(100);
                count++;

                if (count >= 100)
                {
                    if (true == UpdateNodeMap())
                    {
                        // TODO: monitor all tags
                    }

                    count = 0;
                }

                // TODO: read all tags
            }
        }

        public static DaClient ClientInstance
        {
            get
            {
                return _client;
            }
        }

        public static bool IsRunning
        {
            get
            {
                return _clientRunning;
            }
        }

        public static Node GetNode(string name)
        {
            if (_nodeMap.ContainsKey(name))
            {
                return _nodeMap[name];
            }

            return null;
        }

        public static IEnumerable<Node> GetNodes()
        {
            return _nodeMap.Values;
        }


        public static void Start(string serverUrl)
        {
            if (_clientRunning)
            {
                return;
            }

            _client = new DaClient(serverUrl, string.Empty, string.Empty, string.Empty);
            _clientRunning = true;
            _nodeMap = new Dictionary<string, Node>();
            _clientThread = new Thread(new ThreadStart(ClientThread));
            _clientThread.Start();
        }

        public static void Stop()
        {
            if (_clientRunning)
            {
                _clientRunning = false;
                _clientThread.Join();
                _nodeMap = null;
                _clientThread = null;
            }
        }
    }
}

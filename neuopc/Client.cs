using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using neuclient;
using Opc.Ua.Server;
using Serilog;
using neulib;
using System.Threading.Channels;

namespace neuopc
{
    internal class Client
    {
        private static DaClient _client = null;
        private static bool _clientRunning = false;
        private static Thread _clientThread = null;
        private static Dictionary<string, Node> _nodeMap = null;
        private static Channel<Msg> _dataChannel = null;

        private static bool UpdateNodeMap()
        {
            bool upgrade = false;
            var nodes = DaBrowse.AllItemNode(_client.Server);
            foreach (var node in nodes)
            {
                if (!_nodeMap.ContainsKey(node.ItemName))
                {
                    upgrade = true;
                    _nodeMap.Add(node.ItemName, node);
                }
            }

            return upgrade;
        }

        private static void ReadTags()
        {
            int MaxReadCount = 100;
            int count = _nodeMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                var nodes = _nodeMap.Values.Skip(i * MaxReadCount).Take(MaxReadCount);
                var tags = nodes.Select(n => n.ItemName).ToList();
                var items = _client.Read(tags);

                var list = new List<Item>();
                foreach (var item in items)
                {
                    var node = _nodeMap[item.Key];
                    node.Item = item.Value;

                    var it = new Item()
                    {
                        Name = node.ItemName,
                        Type = node.Type,
                        Value = node.Item.Value,
                        Quality = node.Item.Quality,
                        Timestamp = node.Item.SourceTimestamp,
                    };
                    list.Add(it);
                }

                

                _dataChannel.Writer.TryWrite(new Msg()
                {
                    Items = list,
                });
            }
        }

        private static void ClientThread()
        {
            int count = 100;
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

                if (_nodeMap.Count == 0)
                {
                    Thread.Sleep(100);
                }

                count++;
                if (count >= 100)
                {
                    if (true == UpdateNodeMap())
                    {
                        // TODO: monitor all tags
                    }

                    count = 0;
                }

                ReadTags();
            }
        }

        public static DaClient ClientInstance
        {
            get
            {
                return _client;
            }
        }

        public static bool Running
        {
            get
            {
                return _clientRunning;
            }
        }

        public static IEnumerable<Node> GetNodes()
        {
            if (null != _nodeMap)
            {
                return _nodeMap.Values;
            }

            return null;
        }


        public static void Start(string serverUrl, Channel<Msg> dataChannel)
        {
            if (_clientRunning)
            {
                return;
            }

            _client = new DaClient(serverUrl, string.Empty, string.Empty, string.Empty);
            _clientRunning = true;
            _nodeMap = new Dictionary<string, Node>();
            _dataChannel = dataChannel;
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
                _dataChannel = null;
                _clientThread = null;
            }
        }
    }
}

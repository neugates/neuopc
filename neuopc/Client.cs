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
using System.Windows.Forms;
using System.Linq.Expressions;

namespace neuopc
{
    internal class NodeInfo
    {
        public Node Node { get; set; }

        /// <summary>
        /// True if you have been added to the subscription list.
        /// </summary>
        public bool Subscribed { get; set; }
    }

    internal class Client
    {
        private static readonly int MaxReadCount = 100;
        private static DaClient _client = null;
        private static bool _clientRunning = false;
        private static Thread _clientThread = null;
        private static Dictionary<string, NodeInfo> _infoMap = null;
        private static Channel<Msg> _dataChannel = null;

        private static void UpdateNodeMap()
        {
            var nodes = DaBrowse.AllItemNode(_client.Server);
            foreach (var node in nodes)
            {
                if (!_infoMap.ContainsKey(node.ItemName))
                {
                    _infoMap.Add(node.ItemName, new NodeInfo
                    {
                        Node = node,
                        Subscribed = false,
                    });
                }
            }
        }

        private static void ReadTags()
        {
            int count = _infoMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                var nodes = _infoMap.Values.Where(x => !x.Subscribed).Skip(i * MaxReadCount).Take(MaxReadCount);
                var tags = nodes?.Select(n => n.Node.ItemName).ToList();
                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                var items = _client.Read(tags);
                var list = new List<Item>();
                foreach (var item in items)
                {
                    var node = _infoMap[item.Key];
                    node.Node.Item = item.Value;

                    var it = new Item()
                    {
                        Name = node.Node.ItemName,
                        Type = node.Node.Type,
                        Value = node.Node.Item.Value,
                        Quality = node.Node.Item.Quality,
                        Timestamp = node.Node.Item.SourceTimestamp,
                    };
                    list.Add(it);
                }

                _dataChannel.Writer.TryWrite(new Msg()
                {
                    Items = list,
                });
            }
        }

        private static void MonitorTags()
        {
            int count = _infoMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                var nodes = _infoMap.Values.Where(x => !x.Subscribed).Skip(i * MaxReadCount).Take(MaxReadCount);
                var tags = nodes?.Select(n => n.Node.ItemName).ToList();
                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                _client.Monitor(tags, (dic, stop) =>
                {
                    if (false == _clientRunning)
                    {
                        stop();
                        return;
                    }

                    var list = new List<Item>();
                    foreach (var kv in dic)
                    {
                        var info = _infoMap[kv.Key];
                        info.Node.Item = kv.Value;
                        info.Subscribed = true;

                        var it = new Item()
                        {
                            Name = kv.Key,
                            Type = info.Node.Type,
                            Value = kv.Value.Value,
                            Quality = kv.Value.Quality,
                            Timestamp = kv.Value.SourceTimestamp,
                        };
                        list.Add(it);
                    }

                    _dataChannel.Writer.TryWrite(new Msg()
                    {
                        Items = list,
                    });
                });
            }
        }

        private static void ClientThread()
        {
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


                UpdateNodeMap();
                ReadTags();
                MonitorTags();
                Thread.Sleep(1000);
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

        /// <summary>
        /// Get nodes info, use by GUI 
        /// </summary>
        /// <returns>Return null if client not running</returns>
        public static IEnumerable<NodeInfo> GetNodes()
        {
            if (null != _infoMap)
            {
                return _infoMap.Values;
            }

            return null;
        }

        public static bool WriteTag(Item item)
        {
            if (!_clientRunning)
            {
                Log.Error($"Write {item.Name} failed, client not running");
                return false;
            }

            var node = _infoMap[item.Name];
            if (null == node)
            {
                Log.Error($"Tag {item.Name} not found");
                return false;
            }

            Log.Information($"Write {item.Name} = {item.Value}");
            if (null != _client)
            {
                try
                {
                    _client.Write(item.Name, item.Value);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Write {item.Name} failed");
                    return false;
                }
            }

            return true;
        }

        public static void Start(string serverUrl, Channel<Msg> dataChannel)
        {
            if (_clientRunning)
            {
                return;
            }

            _client = new DaClient(serverUrl, string.Empty, string.Empty, string.Empty);
            _clientRunning = true;
            _infoMap = new Dictionary<string, NodeInfo>();
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
                _infoMap = null;
                _dataChannel = null;
                _clientThread = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using neuclient;
using Serilog;
using neulib;
using System.Threading.Channels;
using System.Windows.Forms;
using Opc.Ua.Server;
using System.Text.RegularExpressions;

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

        public static IEnumerable<Node> AllItemNode(Opc.Da.Server server)
        {
            Log.Information("enter get all item node");

            IEnumerable<Node> nodes = null;
            try
            {
                nodes = DaBrowse.AllNode(server);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "browse all node failed");
            }

            var items = nodes.Where(x => x.IsItem);
            foreach (var item in items)
            {
                try
                {
                    item.Type = DaBrowse.GetDataType(server, item.ItemName, item.ItemPath);
                }
                catch (ArgumentOutOfRangeException)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"get data type ArgumentOutOfRangeException, item:{item.ItemName}"
                    );
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "get data type failed");
                }
            }

            return items;
        }

        private static void UpdateNodeMap()
        {
            var nodes = AllItemNode(_client.Server);
            foreach (var node in nodes)
            {
                if (!_infoMap.ContainsKey(node.ItemName))
                {
                    _infoMap.Add(node.ItemName, new NodeInfo { Node = node, Subscribed = false, });
                }
            }
        }

        private static void ReadTags()
        {
            int count = _infoMap.Count;
            //int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);
            int times = count / 1 + ((count % 1) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                var nodes = _infoMap.Values.Where(x => !x.Subscribed).Skip(i * 1).Take(1);
                var tags = nodes?.Select(n => n.Node.ItemName).ToList();
                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                IDictionary<string, ReadItem> items = new Dictionary<string, ReadItem>();
                try
                {
                    items = _client.Read(tags);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "read tags failed");
                    continue;
                }

                foreach (var kv in items)
                {
                    Type type;
                    try
                    {
                        type = DaBrowse.GetDataType(_client.Server, kv.Key, string.Empty);
                        Log.Information($"tag name:{kv.Key}, tag type:{type}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "ta");
                    }
                }

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

                    try
                    {
                        if (it.Type == typeof(byte[]))
                        {
                            it.Type = typeof(byte);
                            it.Value = ((byte[])it.Value)[0];

                            if (it.Value is byte[] ary && 0 < ary.Length)
                            {
                                it.Value = ary[0];
                            }
                            else
                            {
                                it.Value = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "read byte[] error");
                    }

                    list.Add(it);
                }

                _dataChannel.Writer.TryWrite(new Msg() { Items = list, });
            }
        }

        private static void MonitorTags()
        {
            int count = _infoMap.Count;
            int times = count / MaxReadCount + ((count % MaxReadCount) == 0 ? 0 : 1);

            for (int i = 0; i < times; i++)
            {
                var nodes = _infoMap.Values
                    .Where(x => !x.Subscribed)
                    .Skip(i * MaxReadCount)
                    .Take(MaxReadCount);
                var tags = nodes?.Select(n => n.Node.ItemName).ToList();
                if (null == tags || 0 >= tags.Count)
                {
                    continue;
                }

                _client.Monitor(
                    tags,
                    (dic, stop) =>
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

                            try
                            {
                                if (it.Type == typeof(byte[]))
                                {
                                    it.Type = typeof(byte);

                                    if (kv.Value.Value is byte[] ary && 0 < ary.Length)
                                    {
                                        it.Value = ary[0];
                                    }
                                    else
                                    {
                                        it.Value = 0;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "monitor byte[] error");
                            }

                            list.Add(it);
                        }

                        _dataChannel.Writer.TryWrite(new Msg() { Items = list, });
                    }
                );
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

                //UpdateNodeMap();
                ReadTags();
                MonitorTags();
                Thread.Sleep(1000);
            }
        }

        public static DaClient ClientInstance
        {
            get { return _client; }
        }

        public static bool Running
        {
            get { return _clientRunning; }
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

        private static Type MatchType(string type)
        {
            return type switch
            {
                "System.SByte" => typeof(sbyte),
                "System.Int16" => typeof(short),
                "System.Int32" => typeof(int),
                "System.Int64" => typeof(long),
                "System.Single" => typeof(float),
                "System.Double" => typeof(double),
                "System.Byte" => typeof(byte),
                "System.UInt16" => typeof(ushort),
                "System.UInt32" => typeof(uint),
                "System.UInt64" => typeof(ulong),
                "System.DateTime" => typeof(DateTime),
                "System.String" => typeof(string),
                "System.Boolean" => typeof(bool),
                "System.Byte[]" => typeof(byte[]),
                _ => typeof(object),
            };
        }

        private static void LoadTags()
        {
            try
            {
                var tags = TagJson.GetTags("tags.json");
                foreach (var tag in tags)
                {
                    var node = new Node
                    {
                        Name = tag.ItemName,
                        ItemName = tag.ItemName,
                        Type = MatchType(tag.DataType),
                    };
                    _infoMap.Add(node.ItemName, new NodeInfo { Node = node, Subscribed = false, });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "read tags.json error");
            }
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

            LoadTags();

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

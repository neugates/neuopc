using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Globalization;
using System.Net;
using Opc;
using neulib_nf;

namespace neuclient_nf
{
    public class DaClient
    {
        private const int DefaultMonitorInterval = 100;

        private readonly URL _url;

        private readonly string _user;

        private readonly string _password;

        private readonly string _domain;

        private Opc.Da.Server _server;

        private WebProxy _proxy = null;

        private long _subscription;

        public DaClient(string serverUrl, string user, string password, string domain)
        {
            try
            {
                _url = new URL(serverUrl);
            }
            catch (Exception)
            {
                throw;
            }

            _user = user;
            _password = password;
            _domain = domain;
        }

        public ServerStatus Status { get; set; }

        public int? MonitorInterval { get; set; }

        public Opc.Da.Server Server
        {
            get { return _server; }
        }

        public void Connect()
        {
            if (Status == ServerStatus.Connected)
            {
                return;
            }

            _server = new Opc.Da.Server(new OpcCom.Factory(), _url);

            if (!string.IsNullOrEmpty(_user))
            {
                var credential = DaServer.GetNetCredential(_user, _password, _domain);
                var connectData = DaServer.GetConnectData(credential, _proxy);
                _server.Connect(connectData);
            }

            _server.Connect();
            Status = ServerStatus.Connected;
        }

        public void Disconnect()
        {
            if (Status == ServerStatus.Disconnected)
            {
                return;
            }

            _server.Disconnect();
        }

        public System.Type GetDataType(string tag)
        {
            var item = new Opc.Da.Item { ItemName = tag };
            Opc.Da.ItemProperty result;

            try
            {
                var propertyCollection = _server.GetProperties(
                    new ItemIdentifier[] { item },
                    new[] { new Opc.Da.PropertyID(1) },
                    false
                )[0];
                result = propertyCollection[0];
            }
            catch (NullReferenceException)
            {
                throw new Exception("");
            }

            return result.DataType;
        }

        public ReadItem Read(string tag)
        {
            var item = new Opc.Da.Item { ItemName = tag };
            if (Status == ServerStatus.Disconnected)
            {
                throw new Exception("Server not connected. Cannot read tag.");
            }

            var result = _server.Read(new[] { item })[0];
            var readItem = new ReadItem
            {
                Value = result.Value,
                SourceTimestamp = result.Timestamp,
                ServerTimestamp = result.Timestamp
            };
            if (result.Quality == Opc.Da.Quality.Good)
            {
                readItem.Quality = Quality.Good;
            }
            else
            {
                readItem.Quality = (Quality)result.Quality.QualityBits;
            }

            return readItem;
        }

        private static T TryCastResult<T>(object value)
        {
            try
            {
                return (T)value;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException(
                    $"Could not monitor tag. Cast failed for type \"{typeof(T)}\" on the new value \"{value}\" with type \"{value.GetType()}\". Make sure tag data type matches."
                );
            }
        }

        public ReadItem<T> Read<T>(string tag)
        {
            var readItem = Read(tag);
            var readItemGeneric = new ReadItem<T>
            {
                Value = TryCastResult<T>(readItem.Value),
                Quality = readItem.Quality,
                ServerTimestamp = readItem.ServerTimestamp,
                SourceTimestamp = readItem.SourceTimestamp
            };
            return readItemGeneric;
        }

        public async Task<ReadItem<T>> ReadAsync<T>(string tag)
        {
            return await Task.Run(() => Read<T>(tag));
        }

        public IDictionary<string, ReadItem> Read(IEnumerable<string> tags)
        {
            tags = tags?.Distinct().ToArray() ?? new string[0];

            if (!tags.Any())
            {
                return new Dictionary<string, ReadItem>();
            }
            if (Status == ServerStatus.Disconnected)
            {
                throw new Exception("Server not connected. Cannot read tag.");
            }

            IEnumerable<Opc.Da.Item> items =
                from tag in tags
                let item = new Opc.Da.Item { ItemName = tag }
                select item;

            Opc.Da.ItemValueResult[] results = null;
            try
            {
                results = _server.Read(items.ToArray());
            }
            catch (NotConnectedException)
            {
                throw;
            }

            IDictionary<string, ReadItem> readItems = new ConcurrentDictionary<string, ReadItem>();
            foreach (Opc.Da.ItemValueResult result in results)
            {
                var readItem = new ReadItem()
                {
                    Value = result.Value,
                    SourceTimestamp = result.Timestamp,
                    ServerTimestamp = result.Timestamp
                };

                if (result.Quality == Opc.Da.Quality.Good)
                {
                    readItem.Quality = Quality.Good;
                }
                else
                {
                    readItem.Quality = (Quality)result.Quality.QualityBits;
                }

                string tag = result.ItemName.ToString();
                readItems.Add(tag, readItem);
            }

            return readItems;
        }

        public void Write<T>(string tag, T item)
        {
            var itmVal = new Opc.Da.ItemValue { ItemName = tag, Value = item };

            IdentifiedResult result = _server.Write(new[] { itmVal })[0];
            if (result == null)
            {
                throw new Exception("The server replied with an empty response");
            }
            if (result.ResultID.ToString() != "S_OK")
            {
                throw new Exception(
                    $"Invalid response from the server. (Response Status: {result.ResultID}, Opc Tag: {tag})"
                );
            }
        }

        public async Task WriteAsync<T>(string tag, T item)
        {
            await Task.Run(() => Write(tag, item));
        }

        public void Monitor(string tag, Action<ReadItem, Action> callback)
        {
            var subItem = new Opc.Da.SubscriptionState
            {
                Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
                Active = true,
                UpdateRate = MonitorInterval ?? DefaultMonitorInterval
            };

            var sub = _server.CreateSubscription(subItem);
            void unsubscribe() => new Thread(o => _server.CancelSubscription(sub)).Start();
            sub.DataChanged += (handle, requestHandle, values) =>
            {
                var monitorItem = new ReadItem()
                {
                    Value = values[0].Value,
                    SourceTimestamp = values[0].Timestamp,
                    ServerTimestamp = values[0].Timestamp
                };
                if (values[0].Quality == Opc.Da.Quality.Good)
                {
                    monitorItem.Quality = Quality.Good;
                }
                else
                {
                    monitorItem.Quality = (Quality)values[0].Quality.QualityBits;
                }

                callback(monitorItem, unsubscribe);
            };
            sub.AddItems(new[] { new Opc.Da.Item { ItemName = tag } });
            sub.SetEnabled(true);
        }

        public void Monitor<T>(string tag, Action<ReadItem<T>, Action> callback)
        {
            var subItem = new Opc.Da.SubscriptionState
            {
                Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
                Active = true,
                UpdateRate = MonitorInterval ?? DefaultMonitorInterval
            };

            var sub = _server.CreateSubscription(subItem);
            void unsubscribe() => new Thread(o => _server.CancelSubscription(sub)).Start();
            sub.DataChanged += (handle, requestHandle, values) =>
            {
                T casted = TryCastResult<T>(values[0].Value);
                var monitorItem = new ReadItem<T>()
                {
                    Value = casted,
                    SourceTimestamp = values[0].Timestamp,
                    ServerTimestamp = values[0].Timestamp
                };

                if (values[0].Quality == Opc.Da.Quality.Good)
                {
                    monitorItem.Quality = Quality.Good;
                }
                else
                {
                    monitorItem.Quality = (Quality)values[0].Quality.QualityBits;
                }
                callback(monitorItem, unsubscribe);
            };
            sub.AddItems(new[] { new Opc.Da.Item { ItemName = tag } });
            sub.SetEnabled(true);
        }

        public void Monitor(
            IEnumerable<string> tags,
            Action<IDictionary<string, ReadItem>, Action> callback
        )
        {
            tags = tags?.Distinct().ToArray() ?? new string[0];
            if (!tags.Any())
            {
                throw new ArgumentNullException(nameof(tags), "tags cannot be empty !");
            }

            var subItem = new Opc.Da.SubscriptionState
            {
                Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
                Active = true,
                UpdateRate = MonitorInterval ?? DefaultMonitorInterval
            };

            var sub = _server.CreateSubscription(subItem);
            void unsubscribe() =>
                new Thread(o =>
                {
                    try
                    {
                        _server.CancelSubscription(sub);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex);
                    }
                }).Start();

            IDictionary<string, ReadItem> readEvents = new ConcurrentDictionary<string, ReadItem>();
            sub.DataChanged += (handle, requestHandle, values) =>
            {
                foreach (Opc.Da.ItemValueResult itemValueResult in values)
                {
                    var monitorItem = new ReadItem()
                    {
                        Value = itemValueResult.Value,
                        SourceTimestamp = itemValueResult.Timestamp,
                        ServerTimestamp = itemValueResult.Timestamp
                    };

                    if (values[0].Quality == Opc.Da.Quality.Good)
                    {
                        monitorItem.Quality = Quality.Good;
                    }
                    else
                    {
                        monitorItem.Quality = (Quality)values[0].Quality.QualityBits;
                    }

                    string tag = itemValueResult.ItemName.ToString();
                    if (readEvents.ContainsKey(tag))
                    {
                        readEvents[tag] = monitorItem;
                    }
                    else
                    {
                        readEvents.Add(tag, monitorItem);
                    }
                }
                callback(readEvents, (Action)unsubscribe);
            };

            sub.AddItems(tags.Select(tag => new Opc.Da.Item { ItemName = tag }).ToArray());

            try
            {
                sub.SetEnabled(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public void MonitorChanges(
            IEnumerable<string> tags,
            Action<IDictionary<string, ReadItem>, Action> callback
        )
        {
            tags = tags?.Distinct().ToArray() ?? new string[0];
            if (!tags.Any())
            {
                throw new ArgumentNullException(nameof(tags), "tags cannot be empty !");
            }

            var subItem = new Opc.Da.SubscriptionState
            {
                Name = (++_subscription).ToString(CultureInfo.InvariantCulture),
                Active = true,
                UpdateRate = MonitorInterval ?? DefaultMonitorInterval
            };

            var sub = _server.CreateSubscription(subItem);
            void unsubscribe() => new Thread(o => _server.CancelSubscription(sub)).Start();
            sub.DataChanged += (handle, requestHandle, values) =>
            {
                IDictionary<string, ReadItem> readEvents =
                    new ConcurrentDictionary<string, ReadItem>();

                foreach (Opc.Da.ItemValueResult itemValueResult in values)
                {
                    var monitorItem = new ReadItem()
                    {
                        Value = itemValueResult.Value,
                        SourceTimestamp = itemValueResult.Timestamp,
                        ServerTimestamp = itemValueResult.Timestamp
                    };

                    if (values[0].Quality == Opc.Da.Quality.Good)
                    {
                        monitorItem.Quality = Quality.Good;
                    }
                    else
                    {
                        monitorItem.Quality = (Quality)values[0].Quality.QualityBits;
                    }

                    string tag = itemValueResult.ItemName.ToString();
                    readEvents.Add(tag, monitorItem);
                }

                callback(readEvents, (Action)unsubscribe);
            };

            sub.AddItems(tags.Select(tag => new Opc.Da.Item { ItemName = tag }).ToArray());
            sub.SetEnabled(true);
        }

        public void Dispose()
        {
            if (Server != null)
            {
                _server.Dispose();
                Status = ServerStatus.Disconnected;

                GC.SuppressFinalize(this);
            }
        }
    }
}

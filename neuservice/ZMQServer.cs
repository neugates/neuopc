using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using neulib;
using System.Threading;

namespace neuservice
{
    class ZMQServer
    {
        private string uri;
        private bool running;
        private object runningLocker;
        private DAClient client;
        private UAServer server;
        private Thread thread;
        private object timestampLocker;
        private long timestamp;
        private DataNodes nodes;

        public ZMQServer(string uri, DAClient client, UAServer server, DataNodes nodes)
        {
            this.uri = uri;
            this.client = client;
            this.server = server;
            this.nodes = nodes;

            runningLocker = new object();
            timestampLocker = new object();
        }

        public long GetTimeStamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        private void Tiktok()
        {
            while (true)
            {
                lock (runningLocker)
                {
                    if (!running)
                    {
                        break;
                    }
                }

                long timeout = 0;
                lock (timestampLocker)
                {
                    timeout = GetTimeStamp() - timestamp;
                }

                if (timeout > 5)
                {
                    this.client.Close();
                    this.server.Stop();
                    Environment.Exit(0);
                    Log.Information("exit from watchdog");
                }

                Thread.Sleep(1000);
            }
        }

        public void Loop()
        {
            running = true;
            timestamp = GetTimeStamp();
            var ts = new ThreadStart(Tiktok);
            thread = new Thread(ts);
            thread.Start();

            using var responseSocket = new ResponseSocket(uri);
            Log.Information("start zmq server receive loop");

            while (true)
            {
                lock (runningLocker)
                {
                    if (!running)
                    {
                        break;
                    }
                }

                var msg = responseSocket.ReceiveFrameBytes();
                var baseMsg = Serializer.Deserialize<MsgBase>(msg);

                lock (timestampLocker)
                {
                    timestamp = GetTimeStamp();
                }

                switch (baseMsg.type)
                {
                    case neulib.MsgType.Ping:
                        {
                            var pong = new PongMsg();
                            var buff = Serializer.Serialize<PongMsg>(pong);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.DAHostsReq:
                        {
                            var requestMsg = Serializer.Deserialize<DAHostsReqMsg>(msg);
                            var responseMsg = GetDAHosts(requestMsg);
                            var buff = Serializer.Serialize<DAHostsResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.DAServersReq:
                        {
                            break;
                        }
                    case neulib.MsgType.DAConnectReq:
                        {
                            break;
                        }
                    case neulib.MsgType.DADataReq:
                        {
                            var requestMsg = Serializer.Deserialize<DataReqMsg>(msg);
                            var responseMsg = GetItems(requestMsg);
                            var buff = Serializer.Serialize<DataResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.DAStatusReq:
                        {
                            break;
                        }
                    case neulib.MsgType.DADisconnectReq:
                        {
                            break;
                        }
                    case neulib.MsgType.UAStartReq:
                        {
                            break;
                        }
                    case neulib.MsgType.UAStatusReq:
                        {
                            break;
                        }
                    case neulib.MsgType.UAStopReq:
                        {
                            break;
                        }
                    case neulib.MsgType.ExitReq:
                        {
                            running = false;
                            var responseMsg = new ExitResMsg();
                            responseMsg.type = neulib.MsgType.ExitRes;
                            var buff = Serializer.Serialize<ExitResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);

                            break;
                        }
                    default:
                        break;
                }
            }

            Log.Information("end zmq server receive loop");
        }

        private void Enter()
        {
            ConnectDAServer();
            StartUAServer();
        }

        private void Exit()
        {
            DisconnectDAServer();
            StopUAServer();
        }

        private DAHostsResMsg GetDAHosts(DAHostsReqMsg requestMsg)
        {
            var responseMsg = new DAHostsResMsg();
            responseMsg.type = neulib.MsgType.DAHostsRes;
            responseMsg.hosts = DAClient.GetHosts();
            return responseMsg;
        }

        private void GetDAServers() { }

        private void ConnectDAServer() { }

        private DataResMsg GetItems(DataReqMsg requestMsg)
        {
            var list = this.nodes.GetNodes(out long sequence);
            var dataItems = new List<DataItem>();
            foreach (var item in list)
            {
                dataItems.Add(new DataItem
                {
                    Name = item.Name,
                    ClientHandle = item.ClientHandle.ToString(),
                    Right = item.Rights.ToString(),
                    Value = Convert.ToString(item.Value),
                    Quality = item.Quality.ToString(),
                    Error = item.Error.ToString(),
                    Timestamp = Convert.ToString(item.Timestamp)
                });
            }

            return new DataResMsg
            {
                sequence = sequence,
                Items = dataItems
            };
        }

        private void GetDAServerStatus() { }

        private void DisconnectDAServer() { }

        private void StartUAServer() { }

        private void GetUAServerStatus() { }

        private void StopUAServer() { }
    }
}

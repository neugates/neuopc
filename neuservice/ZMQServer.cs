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
        private readonly string uri;
        private bool running;
        private readonly object runningLocker;
        private readonly DAClient client;
        private readonly UAServer server;
        private Thread thread;
        private readonly object timestampLocker;
        private long timestamp;
        private readonly DataNodes nodes;

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

                if (timeout > 30)
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

                switch (baseMsg.Type)
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
                            var requestMsg = Serializer.Deserialize<DAServerReqMsg>(msg);
                            var responseMsg = GetDAServers(requestMsg);
                            var buff = Serializer.Serialize<DAServerResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.DAConnectReq:
                        {
                            var requestMsg = Serializer.Deserialize<ConnectReqMsg>(msg);
                            var responseMsg = ConnectDAServer(requestMsg);
                            var buff = Serializer.Serialize<ConnectResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.DAConnectTestReq:
                        {
                            var requestMsg = Serializer.Deserialize<ConnectTestReqMsg>(msg);
                            var responseMsg = ConnectTest(requestMsg);
                            var buff = Serializer.Serialize<ConnectTestResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
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
                            var requestMsg = Serializer.Deserialize<DisconnectReqMsg>(msg);
                            var responseMsg = DisconnectDAServer(requestMsg);
                            var buff = Serializer.Serialize<DisconnectResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);

                            break;
                        }
                    case neulib.MsgType.UAStartReq:
                        {
                            var requestMsg = Serializer.Deserialize<UAStartReqMsg>(msg);
                            var responseMsg = StartUAServer(requestMsg);
                            var buff = Serializer.Serialize<UAStartResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.UAStatusReq:
                        {
                            break;
                        }
                    case neulib.MsgType.UAStopReq:
                        {
                            var requestMsg = Serializer.Deserialize<UAStopReqMsg>(msg);
                            var responseMsg = StopUAServer(requestMsg);
                            var buff = Serializer.Serialize<UAStopResMsg>(responseMsg);
                            responseSocket.SendFrame(buff, false);
                            break;
                        }
                    case neulib.MsgType.ExitReq:
                        {
                            running = false;
                            var responseMsg = new ExitResMsg();
                            responseMsg.Type = neulib.MsgType.ExitRes;
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
        }

        private void Exit()
        {
        }

        private DAHostsResMsg GetDAHosts(DAHostsReqMsg requestMsg)
        {
            var responseMsg = new DAHostsResMsg();
            responseMsg.Type = neulib.MsgType.DAHostsRes;
            responseMsg.Hosts = DAClient.GetHosts();
            return responseMsg;
        }

        private DAServerResMsg GetDAServers(DAServerReqMsg requestMsg)
        {
            var responseMsg = new DAServerResMsg();
            responseMsg.Type = neulib.MsgType.DAServersRes;
            responseMsg.Servers = DAClient.GetServers(requestMsg.Host);
            return responseMsg;
        }

        private ConnectResMsg ConnectDAServer(ConnectReqMsg requestMsg)
        {
            var responseMsg = new ConnectResMsg();
            responseMsg.Type = neulib.MsgType.DAConnectRes;
            client.Open(requestMsg.Host, requestMsg.Server);
            return responseMsg;
        }

        private ConnectTestResMsg ConnectTest(ConnectTestReqMsg requestMsg)
        {
            var responseMsg = new ConnectTestResMsg();
            responseMsg.Type = neulib.MsgType.DAConnectTestRes;
            responseMsg.Result = client.TestServer(requestMsg.Host, requestMsg.Server);
            return responseMsg;
        }


        private DataResMsg GetItems(DataReqMsg requestMsg)
        {
            var list = this.nodes.GetNodes();
            var dataItems = new List<DataItem>();
            foreach (var item in list)
            {
                dataItems.Add(new DataItem
                {
                    Name = item.Name,
                    Type = item.Type.ToString(),
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
                Items = dataItems
            };
        }

        private void GetDAServerStatus() { }

        private DisconnectResMsg DisconnectDAServer(DisconnectReqMsg requestMsg)
        {
            var responseMsg = new DisconnectResMsg();
            responseMsg.Type = neulib.MsgType.DADisconnectRes;
            client.Close();
            return responseMsg;
        }

        private UAStartResMsg StartUAServer(UAStartReqMsg requestMsg)
        {
            var responseMsg = new UAStartResMsg();
            responseMsg.Type = neulib.MsgType.UAStartRes;
            server.Start(requestMsg.Url, requestMsg.User, requestMsg.Password);
            return responseMsg;
        }

        private void GetUAServerStatus() { }

        private UAStopResMsg StopUAServer(UAStopReqMsg requestMsg)
        {
            var responseMsg = new UAStopResMsg();
            responseMsg.Type = neulib.MsgType.UAStopRes;
            server.Stop();
            return responseMsg;
        }
    }
}

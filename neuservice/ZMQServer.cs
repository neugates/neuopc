using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using neulib;

namespace neuservice
{
    class ZMQServer
    {
        private string uri;
        private bool running;
        private DAClient client;
        private UAServer server;

        public ZMQServer(string uri, DAClient client, UAServer server)
        {
            this.uri = uri;
            this.client = client;
            this.server = server;
        }

        public void Loop()
        {
            running = true;
            using var responseSocket = new ResponseSocket(uri);
            Log.Information("start zmq server receive loop");

            while (running)
            {
                var msg = responseSocket.ReceiveFrameBytes();
                var baseMsg = Serializer.Deserialize<MsgBase>(msg);

                switch (baseMsg.type)
                {
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

        private void GetDAServerStatus() { }

        private void DisconnectDAServer() { }

        private void StartUAServer() { }

        private void GetUAServerStatus() { }

        private void StopUAServer() { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

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
            running = false;

            using (var responseSocket = new ResponseSocket(uri))
            {
                while (!running)
                {
                    var msg = responseSocket.ReceiveFrameBytes();
                    // TODO: if msg is stop then call Stop()
                }
            }
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

        private void GetDAHosts() { }

        private void GetDAServers() { }

        private void ConnectDAServer() { }

        private void GetDAServerStatus() { }

        private void DisconnectDAServer() { }

        private void StartUAServer() { }

        private void GetUAServerStatus() { }

        private void StopUAServer() { }
    }
}

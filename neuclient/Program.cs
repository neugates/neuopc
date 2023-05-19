using System;
using NetMQ;
using NetMQ.Sockets;

namespace neuclient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using (var requestSocket = new RequestSocket(">tcp://localhost:5555"))
            {
                NetMQMessage msg = new NetMQMessage();
            }
        }
    }
}

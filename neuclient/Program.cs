using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using neulib;

namespace neuclient
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("neuservice start...");

            //using var requestSocket = new RequestSocket(">tcp://localhost:5555");
            //var req = new DAHostsReqMsg();
            //req.type = neulib.MsgType.DAHostsReq;
            //var buff = Serializer.Serialize<DAHostsReqMsg>(req);

            //TimeSpan ts = new TimeSpan(0, 0, 20);
            //requestSocket.TrySendFrame(ts, buff, false);
            //byte[] x = new byte[10240];
            //requestSocket.TryReceiveFrameBytes(ts, out x);

            //var daHostMsg = Serializer.Deserialize<DAHostsResMsg>(x);
            //foreach (var host in daHostMsg.hosts)
            //{
            //    System.Console.WriteLine($"->{host}");
            //}
            var subProccess = new SubProcess();
            subProccess.Daemon();

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape);

            subProccess.Stop();
        }
    }
}

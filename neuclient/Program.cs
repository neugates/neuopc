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
        private static bool running = true;

        private static SubProcess subProccess = new SubProcess();

        private static void TestGetDatas()
        {
            while (running)
            {
                Thread.Sleep(3000);

                var req = new DataReqMsg();
                req.Type = neulib.MsgType.DADataReq;
                var buff = Serializer.Serialize<DataReqMsg>(req);
                subProccess.Request(in buff, out byte[] result);

                if (null != result)
                {
                    try
                    {
                        var requestMsg = Serializer.Deserialize<DataResMsg>(result);
                        foreach (var item in requestMsg.Items)
                        {
                            Log.Information($"name:{item.Name}, handle:{item.ClientHandle}, right:{item.Right}, value:{item.Value}, quality:{item.Quality}, error:{item.Error}, timestamp:{item.Timestamp}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"------------->{ex.Message}");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("neuservice start...");

            subProccess.Daemon();

            var ts = new ThreadStart(TestGetDatas);
            var thread = new Thread(ts);
            thread.Start();


            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape);

            running = false;
            subProccess.Stop();
        }
    }
}

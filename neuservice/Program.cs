using System;
using Serilog;

namespace neuservice
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log/neuservice.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("neuservice start...");

            Register.Setup();

            var client = new DAClient();
            var server = new UAServer();

            for (int i = 0; i < args.Length; i++)
            {
                Log.Information($"arg{i}:{args[i]}");
            }

            if (5 < args.Length)
            {
                client.Open(args[0], args[1]);
                server.Start(args[2], args[3], args[4]);
            }

            var listenUri = "@tcp://*:5555";
            if (6 == args.Length)
            {
                listenUri = args[5];
            }

            var zmq = new ZMQServer(listenUri, client, server);
            zmq.Loop();

            client.Close();
            Log.Information("da client close");

            server.Stop();
            Log.Information("ua server stop");

            Log.Information("neuservice quit...");
            Log.CloseAndFlush();
        }
    }
}

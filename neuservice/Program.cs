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

            if (5 == args.Length)
            {
                Log.Information($"arg1:{args[0]}");
                Log.Information($"arg2:{args[1]}");
                Log.Information($"arg3:{args[2]}");
                Log.Information($"arg4:{args[3]}");
                Log.Information($"arg5:{args[4]}");

                client.Open(args[0], args[1]);
                server.Start(args[2], args[3], args[4]);
            }

            var zmq = new ZMQServer("@tcp://*:5555", client, server);
            zmq.Loop();

            Log.Information("neuservice quit...");
            Log.CloseAndFlush();
        }
    }
}

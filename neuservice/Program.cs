using System;
using Serilog;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace neuservice
{
    public class DataNodes
    {
        private readonly object locker;
        private List<Item> nodes;
        private long sequence;

        public DataNodes()
        {
            locker = new object();
            nodes = new List<Item>();
            sequence = 0;
        }

        public void ResetNodes(List<Item> items)
        {
            lock (locker)
            {
                nodes.Clear();
                foreach (var item in items)
                {
                    nodes.Add(item);
                }

                sequence++;
            }
        }

        public void UpdateNodes(List<Item> items)
        {
            lock (locker)
            {
                foreach (var item in items)
                {
                    int index = item.ClientHandle;
                    try
                    {
                        nodes[index].Value = item.Value;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"update node error:{ex.Message}");
                    }
                }
            }
        }

        public List<Item> GetNodes(out long sequence)
        {
            lock (locker)
            {
                sequence = this.sequence;
                return nodes;
            }
        }
    }

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

            var nodes = new DataNodes();
            Channel<DaMsg> channel = Channel.CreateUnbounded<DaMsg>();
            Task task = new Task(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var msg))
                    {
                        if (MsgType.List == msg.Type)
                        {
                            nodes.ResetNodes(msg.Items);
                            Log.Information("reset nodes ");
                        }
                        else if (MsgType.Data == msg.Type)
                        {
                            nodes.UpdateNodes(msg.Items);
                        }
                    }
                }
            });
            task.Start();

            var client = new DAClient();
            var server = new UAServer();
            client.AddFastChannel(server.channel);
            client.AddFastChannel(channel);

            var daHost = "";
            var daServer = "";
            var uaUri = "";
            var uaUser = "";
            var uaPassword = "";
            var zmqUri = "@tcp://*:5555";

            for (int i = 0; i < args.Length; i++)
            {
                Log.Information($"arg{i}:{args[i]}");

                if (!string.IsNullOrEmpty(args[i]))
                {
                    var arg = args[i].Split('=');
                    if (2 == arg.Length)
                    {
                        switch (arg[0])
                        {
                            case "da_host":
                                daHost = arg[1];
                                break;
                            case "da_server":
                                daServer = arg[1];
                                break;
                            case "ua_url":
                                uaUri = arg[1];
                                break;
                            case "ua_user":
                                uaUser = arg[1];
                                break;
                            case "ua_password":
                                uaPassword = arg[1];
                                break;
                            case "zmq_uri":
                                zmqUri = arg[1];
                                break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(daHost) && !string.IsNullOrEmpty(daServer))
            {
                client.Open(daHost, daServer);
            }

            if (!string.IsNullOrEmpty(uaUri) && !string.IsNullOrEmpty(uaUser) && !string.IsNullOrEmpty(uaPassword))
            {
                server.Start(uaUri, uaUser, uaPassword);
            }

            var zmq = new ZMQServer(zmqUri, client, server, nodes);
            zmq.Loop();

            client.Close();
            Log.Information("da client close");

            server.Stop();
            Log.Information("ua server stop");

            channel.Writer.Complete();
            task.Wait();
            Log.Information("end data update task");

            Log.Information("neuservice quit...");
            Log.CloseAndFlush();
        }
    }
}

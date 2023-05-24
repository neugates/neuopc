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

            for (int i = 0; i < args.Length; i++)
            {
                Log.Information($"arg{i}:{args[i]}");
            }

            var listenUri = "@tcp://*:5555";
            if (1 <= args.Length)
            {
                listenUri = args[0];
            }

            if (3 <= args.Length)
            {
                client.Open(args[1], args[2]);
            }

            if (6 <= args.Length)
            {
                server.Start(args[3], args[4], args[5]);
            }

            var zmq = new ZMQServer(listenUri, client, server, nodes);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using neulib;

namespace neuclient
{
    public class NeuServiceInfo
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string ListenUri { get; set; }
        public string ConnectUri { get; set; }
        public string DAHost { get; set; }
        public string DAServer { get; set; }
        public int UAPort { get; set; }
        public string UAUsername { get; set; }
        public string UAPassword { get; set; }
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }
    }


    public class SubProcess
    {
        private RequestSocket requestSocket;
        private ProcessInfo processInfo = null;
        private bool running = true;
        private object runningLocker;
        private object socketLocker;
        private Thread thread;

        public SubProcess()
        {
            runningLocker = new object();
            socketLocker = new object();
            processInfo = new ProcessInfo();
        }

        private IList PortIsUsed()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IList allPorts = new ArrayList();

            foreach (IPEndPoint ep in ipsTCP)
            {
                allPorts.Add(ep.Port);
            }

            foreach (IPEndPoint ep in ipsUDP)
            {
                allPorts.Add(ep.Port);
            }

            foreach (TcpConnectionInformation conn in tcpConnInfoArray)
            {
                allPorts.Add(conn.LocalEndPoint.Port);
            }

            return allPorts;
        }

        private int GetRandomPort()
        {
            IList HasUsedPort = PortIsUsed();
            int port = 0;
            bool IsRandomOk = true;
            Random random = new Random((int)DateTime.Now.Ticks);
            while (IsRandomOk)
            {
                port = random.Next(5555, 65535);
                IsRandomOk = HasUsedPort.Contains(port);
            }

            return port;
        }

        private NeuServiceInfo CreateNeuServiceInfo()
        {
            int port = GetRandomPort();
            var zmqListenUri = $"@tcp://*:{port}";
            var zmqConnectUri = $">tcp://localhost:{port}";

            return new NeuServiceInfo
            {
                Name = "neuservice",
                Path = @"C:\Users\0x1c\workbench\neuopc\neuservice\bin\x86\Debug\netcoreapp3.1\neuservice.exe",
                ListenUri = zmqListenUri,
                ConnectUri = zmqConnectUri,
                DAHost = "kubernetes.docker.internal",
                DAServer = "Intellution.IntellutionGatewayOPCServer",
                UAPort = 48401,
                UAUsername = "admin",
                UAPassword = "123456"
            };
        }


        private void CreateNeuService(NeuServiceInfo serviceInfo)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\0x1c\workbench\neuopc\neuservice\bin\x86\Debug\netcoreapp3.1\neuservice.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    Arguments = $"{serviceInfo.DAHost} {serviceInfo.DAServer} {serviceInfo.UAPort} {serviceInfo.UAUsername} {serviceInfo.UAPassword} {serviceInfo.ListenUri}",
                }
            };

            process.Start();
            processInfo.ProcessId = process.Id;
            Log.Information($"new neuservice process id:{processInfo.ProcessId}");

            lock (socketLocker)
            {
                if (null != requestSocket)
                {
                    try
                    {
                        requestSocket.Close();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"close old request socket error:{ex.Message}");
                    }
                }

                try
                {
                    requestSocket = new RequestSocket(serviceInfo.ConnectUri);
                }
                catch (Exception ex)
                {
                    requestSocket = null;
                    Log.Error($"create new request socket error:{ex.Message}");
                }

                if (null != requestSocket)
                {
                    Log.Information($"create new request socket success, {serviceInfo.ConnectUri}");
                }
                else
                {
                    Log.Fatal($"create new request socket fail, {serviceInfo.ConnectUri}");
                }
            }
        }

        private void Refresh()
        {
            var serviceInfo = CreateNeuServiceInfo();
            CreateNeuService(serviceInfo);

            Log.Information("daemon start");

            while (true)
            {
                lock (runningLocker)
                {
                    if (!running)
                    {
                        break;
                    }
                }

                Thread.Sleep(1000);
                try
                {
                    Process[] processIds = Process.GetProcessesByName("neuservice");
                    if (processIds.Where(n => n.Id == processInfo.ProcessId).Count() > 0)
                    {
                        //Log.Information("neuservice normal");
                    }
                    else
                    {
                        Log.Information("neuservice interrupted, restart");
                        serviceInfo = CreateNeuServiceInfo();
                        CreateNeuService(serviceInfo);
                        Log.Information("neuservice restart success");
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"daemon loop error:{ex.Message}");
                }
            }

            try
            {
                var req = new ExitReqMsg();
                req.type = neulib.MsgType.ExitReq;
                var buff = Serializer.Serialize<ExitReqMsg>(req);
                var result = new byte[100];
                Request(in buff, out result);
                Log.Information("send exit req msg to neuservice, wait to exit");

                var process = Process.GetProcessById(processInfo.ProcessId);
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Warning($"kill neuservice error:{ex.Message}");
            }

            Log.Information("deamon stop");
        }

        public void Daemon()
        {
            var ts = new ThreadStart(Refresh);
            thread = new Thread(ts);
            thread.Start();
        }

        public bool Request(in byte[] send, out byte[] result)
        {
            try
            {
                TimeSpan ts = new TimeSpan(0, 0, 1);
                requestSocket.TrySendFrame(ts, send, false);
                requestSocket.TryReceiveFrameBytes(ts, out result);
            }
            catch (Exception ex)
            {
                Log.Error($"send data to neuservice error:{ex.Message}");
                result = null;
                return false;
            }

            return true;
        }

        public void Stop()
        {
            lock (runningLocker)
            {
                running = false;
            }

            thread?.Join();
        }
    }
}

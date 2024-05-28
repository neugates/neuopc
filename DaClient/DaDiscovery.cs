using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using Opc;
using Opc.Da;
using OpcCom;

namespace DaClient
{
    public class DaDiscovery
    {
        public static IEnumerable<string> GetHosts()
        {
            var hosts = new List<string>();
            var host = Dns.GetHostEntry("127.0.0.1");
            hosts.Add(host.HostName);
            return hosts;
        }

        public static string FixedUrl(string url)
        {
            int index = url.IndexOf("/{");
            if (-1 == index)
            {
                return url;
            }

            return url.Substring(0, index);
        }

        public static IEnumerable<string> GetServers(string host, int version)
        {
            var allServer = new List<string>();
            var discovery = new ServerEnumerator();
            var spec = Specification.COM_DA_20;
            if (1 == version)
            {
                spec = Specification.COM_DA_10;
            }
            else if (2 == version)
            {
                spec = Specification.COM_DA_20;
            }
            else if (3 == version)
            {
                spec = Specification.COM_DA_30;
            }

            var servers = discovery.GetAvailableServers(spec, host, null);
            if (null != servers)
            {
                allServer.AddRange(servers.Where(x => null != x).Select(x => $"{x.Url}"));
            }

            return allServer;
        }
    }
}

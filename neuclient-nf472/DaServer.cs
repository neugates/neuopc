using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Opc;

namespace neuclient_nf
{
    public class DaServer
    {
        public static NetworkCredential GetNetCredential(
            string user,
            string password,
            string domain
        )
        {
            if (null == user || null == password)
            {
                throw new ArgumentNullException("netcredential user or password null");
            }

            if (string.IsNullOrEmpty(domain))
            {
                return new NetworkCredential(user, password);
            }

            return new NetworkCredential(user, password, domain);
        }

        public static ConnectData GetConnectData(NetworkCredential credential, WebProxy proxy)
        {
            return new ConnectData(credential, proxy);
        }
    }
}

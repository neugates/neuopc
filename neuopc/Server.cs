using System.Threading.Channels;
using neuserver;
using neulib;

namespace neuopc
{
    internal class Server
    {
        private static UaServer _server = null;

        public static UaServer ServerInstance
        {
            get
            {
                return _server;
            }
        }

        public static bool Running
        {
            get
            {
                if (_server == null)
                {
                    return false;
                }
                else
                {
                    return _server.Running;
                }
            }
        }

        public static Channel<Msg> DataChannel
        {
            get
            {
                if (_server == null)
                {
                    return null;
                }
                else
                {
                    return _server.DataChannel;
                }
            }
        }   

        public static void Start(string uri, string user, string password, ValueWrite write)
        {
            if (_server == null)
            {
                _server = new UaServer(uri, user, password, write);
                _server.Start();
            }
        }

        public static void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }
    }
}

using System;
using Opc;

namespace neuclient
{
    public class DaClient
    {
        private readonly URL _url;

        private Opc.Da.Server _server;

        public DaClient(Uri serverUrl)
        {
            _url = new URL(serverUrl.AbsolutePath)
            {
                Scheme = serverUrl.Scheme,
                HostName = serverUrl.Host
            };
        }

        public ServerStatus Status { get; }

        public Opc.Da.Server Server { get { return _server; } }

        public void Connect() { }

        public System.Type GetDataType(string tag)
        {
            Opc.Da.Item item = new Opc.Da.Item { ItemName = tag };
            Opc.Da.ItemProperty result;

            try
            {
                Opc.Da.ItemPropertyCollection propertyCollection = _server.GetProperties(new ItemIdentifier[] { item }, new[] { new Opc.Da.PropertyID(1) }, false)[0];
                result = propertyCollection[0];
            }
            catch(NullReferenceException)
            {
                throw new Exception("");
            }

            return result.DataType;
        }
    }
}


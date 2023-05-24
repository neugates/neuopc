using System;
using System.Collections.Generic;
using System.Text;

namespace neulib
{
    [Serializable]
    public class ConnectReqMsg : MsgBase
    {
        public string Host;
        public string Server;
    }

    [Serializable]
    public class ConnectResMsg : MsgBase
    {
        public int Code;
        public string Msg;
    }

    public class DisconnectReqMsg : MsgBase { }

    public class DisconnectResMsg : MsgBase { }
}

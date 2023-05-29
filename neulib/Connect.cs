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

    [Serializable]
    public class ConnectTestReqMsg : MsgBase
    {
        public string Host;
        public string Server;
    }

    [Serializable]
    public class ConnectTestResMsg : MsgBase
    {
        public bool Result;
    }

    [Serializable]
    public class DisconnectReqMsg : MsgBase { }


    [Serializable]
    public class DisconnectResMsg : MsgBase { }
}

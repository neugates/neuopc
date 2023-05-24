using System;
using System.Collections.Generic;
using System.Text;

namespace neulib
{
    [Serializable]
    public class DAServerReqMsg: MsgBase
    {
        public string Host;
    }

    [Serializable]
    public class DAServerResMsg: MsgBase
    {
        public List<string> Servers;
    }

    [Serializable]
    public class DAStatusReqMsg: MsgBase { }


    [Serializable]
    public class DAStatusResMsg: MsgBase { }
}

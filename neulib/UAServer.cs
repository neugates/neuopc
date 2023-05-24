using System;
using System.Collections.Generic;
using System.Text;

namespace neulib
{
    [Serializable]
    public class UAStartReqMsg : MsgBase
    {
        public string Port;
        public string User;
        public string Password;
    }

    [Serializable]
    public class UAStartResMsg : MsgBase
    { }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace neulib
{
    [Serializable]
    public class DAHostsReqMsg : MsgBase
    {
    }

    [Serializable]
    public class DAHostsResMsg : MsgBase
    {
        public List<string> Hosts;
    }
}

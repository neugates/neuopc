using System;
using System.Collections.Generic;
using System.Text;

namespace neulib
{
    [Serializable]
    public class DataReqMsg : MsgBase
    {
    }

    [Serializable]
    public class DataItem {
        public string Name;
        public string Type;
        public string Right;
        public string Value;
        public string Quality;
        public string Error;
        public string Timestamp;
        public string ClientHandle;
    }

    [Serializable]
    public class DataResMsg : MsgBase
    {
        public long Sequence;
        public List<DataItem> Items;
    }
}

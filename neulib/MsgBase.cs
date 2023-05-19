using System;

namespace neulib
{
    public enum MsgType
    {
        DAHostsReq,
        DAHostsRes,
        DAServersReq,
        DAServersRes,
        DAConnectReq,
        DAConnectRes,
        DAStatusReq,
        DAStatusRes,
        DADisconnectReq,
        DADisconnectRes,
        UAStartReq,
        UAStartRes,
        UAStatusReq,
        UAStatusRes,
        UAStopReq,
        UAStopRes,
    }

    [Serializable]
    public class MsgBase
    {
        public MsgType type;
    }
}

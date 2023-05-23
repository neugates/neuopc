using System;

namespace neulib
{
    public enum MsgType
    {
        Ping,
        Pong,
        DAHostsReq,
        DAHostsRes,
        DAServersReq,
        DAServersRes,
        DAConnectReq,
        DAConnectRes,
        DAStatusReq,
        DAStatusRes,
        DADataReq,
        DADataRes,
        DADisconnectReq,
        DADisconnectRes,
        UAStartReq,
        UAStartRes,
        UAStatusReq,
        UAStatusRes,
        UAStopReq,
        UAStopRes,
        ExitReq,
        ExitRes,
    }

    public enum MsgError {
        MsgInvalid,
        MsgSequenceError,
    }

    [Serializable]
    public class MsgBase
    {
        public MsgType type;
        public uint sequence;
        public int error;
    }
}

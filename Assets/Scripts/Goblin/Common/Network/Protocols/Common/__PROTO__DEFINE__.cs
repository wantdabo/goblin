using System;
using System.Collections.Generic;

namespace Queen.Protocols.Common
{
    public partial class ProtoPack
    {
        /// <summary>
        /// 协议号定义
        /// </summary>
        private static Dictionary<ushort, Type> messageDict = new()
        {
            { 10001, typeof(Queen.Protocols.S2CHeartbeatMsg)},
            { 10002, typeof(Queen.Protocols.C2SLogoutMsg)},
            { 10003, typeof(Queen.Protocols.C2SLoginMsg)},
            { 10004, typeof(Queen.Protocols.C2SRegisterMsg)},
            { 10005, typeof(Queen.Protocols.S2CLogoutMsg)},
            { 10006, typeof(Queen.Protocols.S2CLoginMsg)},
            { 10007, typeof(Queen.Protocols.S2CRoleJoinedMsg)},
            { 10008, typeof(Queen.Protocols.S2CRegisterMsg)},
            { 10009, typeof(Queen.Protocols.C2STestMsg)},
            { 10010, typeof(Queen.Protocols.S2CTestMsg)},
            { 10011, typeof(Queen.Protocols.Common.ReqCrossMessage)},
            { 10012, typeof(Queen.Protocols.Common.ResCrossMessage)},
            { 10013, typeof(Queen.Protocols.Common.NodeErrorMsg)},
            { 10014, typeof(Queen.Protocols.Common.NodePingMsg)},
        };
    }
}


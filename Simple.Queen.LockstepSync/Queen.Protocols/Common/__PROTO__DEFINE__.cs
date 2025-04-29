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
            { 10001, typeof(Queen.Protocols.C2SPlayerInputMsg)},
            { 10002, typeof(Queen.Protocols.S2CGameFrameMsg)},
            { 10003, typeof(Queen.Protocols.S2CHeartbeatMsg)},
            { 10004, typeof(Queen.Protocols.C2SLogoutMsg)},
            { 10005, typeof(Queen.Protocols.C2SLoginMsg)},
            { 10006, typeof(Queen.Protocols.C2SRegisterMsg)},
            { 10007, typeof(Queen.Protocols.S2CLogoutMsg)},
            { 10008, typeof(Queen.Protocols.S2CLoginMsg)},
            { 10009, typeof(Queen.Protocols.S2CRoleJoinedMsg)},
            { 10010, typeof(Queen.Protocols.S2CRegisterMsg)},
            { 10011, typeof(Queen.Protocols.C2SStartMatchingMsg)},
            { 10012, typeof(Queen.Protocols.S2CStartMatchingMsg)},
            { 10013, typeof(Queen.Protocols.C2SEndMatchingMsg)},
            { 10014, typeof(Queen.Protocols.S2CEndMatchingMsg)},
            { 10015, typeof(Queen.Protocols.S2CStartGameMsg)},
            { 10016, typeof(Queen.Protocols.C2STestMsg)},
            { 10017, typeof(Queen.Protocols.S2CTestMsg)},
            { 10018, typeof(Queen.Protocols.Common.ACKCrossMessage)},
            { 10019, typeof(Queen.Protocols.Common.ReqCrossMessage)},
            { 10020, typeof(Queen.Protocols.Common.ResCrossMessage)},
            { 10021, typeof(Queen.Protocols.Common.NodeErrorMsg)},
            { 10022, typeof(Queen.Protocols.Common.NodePingMsg)},
        };
    }
}


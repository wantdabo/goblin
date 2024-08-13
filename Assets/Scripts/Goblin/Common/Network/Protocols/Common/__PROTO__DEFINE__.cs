using System;
using System.Collections.Generic;

namespace Queen.Protocols.Common
{
    public partial class ProtoPack
    {
        /// <summary>
        /// 协议号定义
        /// </summary>
        private static Dictionary<uint, Type> messageDict = new()
        {
            { 10002, typeof(Queen.Protocols.C2SLogoutMsg)},
            { 10003, typeof(Queen.Protocols.C2SLoginMsg)},
            { 10004, typeof(Queen.Protocols.C2SRegisterMsg)},
            { 10005, typeof(Queen.Protocols.S2CLogoutMsg)},
            { 10006, typeof(Queen.Protocols.S2CLoginMsg)},
            { 10007, typeof(Queen.Protocols.S2CRegisterMsg)},
            { 10031, typeof(Queen.Protocols.Common.NodePingMsg)},
        };
    }
}


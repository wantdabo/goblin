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
            { 10014, typeof(Queen.Protocols.Common.NodeErrorMsg)},
            { 10015, typeof(Queen.Protocols.Common.NodePingMsg)},
        };
    }
}


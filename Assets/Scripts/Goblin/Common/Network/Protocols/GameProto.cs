using MessagePack;
using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Protocols
{
    /// <summary>
    /// 响应游戏开局消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_GameInfoMsg : INetMessage
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public ushort port { get; set; }
        /// <summary>
        /// 对局房间 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 座位
        /// </summary>
        public uint seat { get; set; }
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 座位，密码
        /// </summary>
        public string password { get; set; }
    }
}

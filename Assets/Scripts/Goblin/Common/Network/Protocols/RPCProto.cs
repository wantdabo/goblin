using MessagePack;
using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Protocols
{
    [MessagePackObject(true)]
    public class S2G_CreateStageMsg : INetMessage
    {
        /// <summary>
        /// 对局房间 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 对局房间名字
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 座位信息列表
        /// </summary>
        public SeatInfo[] seats { get; set; }
    }

    [MessagePackObject(true)]
    public class G2S_CreateStageMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 创建成功，2 房间 ID 重复
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 对局房间 ID
        /// </summary>
        public uint id { get; set; }
    }
}

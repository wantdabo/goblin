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
    /// 请求开始游戏消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2G_StartStageMsg : INetMessage
    {
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

    /// <summary>
    /// 响应开始游戏消息
    /// </summary>
    [MessagePackObject(true)]
    public class G2C_StartStageMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 开始成功，2 身份验证异常，3 座位异常
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 对局房间 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 座位
        /// </summary>
        public uint seat { get; set; }
        /// <summary>
        /// 最大帧
        /// </summary>
        public uint mframe { get; set; }
        /// <summary>
        /// 座位信息列表
        /// </summary>
        public SeatInfo[] seatInfos { get; set; }
        /// <summary>
        /// 历史帧
        /// </summary>
        public FrameInfo[] frameInfos { get; set; }
    }

    /// <summary>
    /// 请求上传操作消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2G_SetInputMsg : INetMessage
    {
        /// <summary>
        /// 输入
        /// </summary>
        public SeatInputInfo inputInfo { get; set; }
    }

    /// <summary>
    /// 响应逻辑驱动消息
    /// </summary>
    [MessagePackObject(true)]
    public class G2C_LogicTickMsg : INetMessage
    {
        /// <summary>
        /// 最新帧
        /// </summary>
        public FrameInfo frameInfo { get; set; }
    }

    /// <summary>
    /// 帧信息
    /// </summary>
    [MessagePackObject(true)]
    public class FrameInfo
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 玩家输入
        /// </summary>
        public SeatInputInfo[] seatInputInfos { get; set; }
    }

    /// <summary>
    /// 输入信息
    /// </summary>
    [MessagePackObject(true)]
    public class SeatInputInfo
    {
        /// <summary>
        /// 座位
        /// </summary>
        public uint seat { get; set; }
        /// <summary>
        /// 移动方向 X
        /// </summary>
        public int moveX { get; set; }
    }

    /// <summary>
    /// 座位信息
    /// </summary>
    [MessagePackObject(true)]
    public class SeatInfo
    {
        /// <summary>
        /// 座位 ID
        /// </summary>
        public uint seat { get; set; }
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 座位，密码
        /// </summary>
        public string password { get; set; }
    }
}

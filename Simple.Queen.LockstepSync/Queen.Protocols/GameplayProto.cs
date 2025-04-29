using MessagePack;
using Queen.Protocols.Common;

namespace Queen.Protocols
{
    /// <summary>
    /// 请求玩家输入消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SPlayerInputMsg : INetMessage
    {
        /// <summary>
        /// Game ID
        /// </summary>
        public ulong id { get; set; }
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; set; }
        /// <summary>
        /// 玩家输入数据列表
        /// </summary>
        public PlayerInputData[] inputs { get; set; }
    }
    
    /// <summary>
    /// 响应游戏帧消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CGameFrameMsg : INetMessage
    {
        /// <summary>
        /// 帧数据
        /// </summary>
        public FrameData frame { get; set; }
        /// <summary>
        /// 帧数据列表 (重连/补发帧数据)
        /// </summary>
        public FrameData[] frames { get; set; }
    }

    /// <summary>
    /// 帧数据
    /// </summary>
    [MessagePackObject(true)]
    public class FrameData
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 输入列表
        /// </summary>
        public PlayerInputData[] inputs { get; set; }
    }

    /// <summary>
    /// 玩家输入数据
    /// </summary>
    [MessagePackObject(true)]
    public class PlayerInputData
    {
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; set; }
        /// <summary>
        /// 输入类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 是否按下
        /// </summary>
        public bool press { get; set; }
        /// <summary>
        /// 方向向量
        /// </summary>
        public Vector2 dire { get; set; }
    }
    
    /// <summary>
    /// 二维向量
    /// </summary>
    [MessagePackObject(true)]
    public class Vector2
    {
        /// <summary>
        /// X 坐标
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        public int y { get; set; }
    }
    
    /// <summary>
    /// 三维向量
    /// </summary>
    [MessagePackObject(true)]
    public class Vector3
    {
        /// <summary>
        /// X 坐标
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// Z 坐标
        /// </summary>
        public int z { get; set; }
    }
}
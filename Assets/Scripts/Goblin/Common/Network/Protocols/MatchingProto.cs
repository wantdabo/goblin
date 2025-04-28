using MessagePack;
using Queen.Protocols.Common;

namespace Queen.Protocols
{
    /// <summary>
    /// 请求开始匹配消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SStartMatchingMsg : INetMessage
    {
        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }
    }

    /// <summary>
    /// 响应开始匹配消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CStartMatchingMsg : INetMessage
    {
    }

    /// <summary>
    /// 请求结束匹配消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SEndMatchingMsg : INetMessage
    {
    }

    /// <summary>
    /// 响应结束匹配消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CEndMatchingMsg : INetMessage
    {
    }

    /// <summary>
    /// 响应开始游戏消息
    /// </summary>
    public class S2CStartGameMsg : INetMessage
    {
        /// <summary>
        /// 我的座位
        /// </summary>
        public ulong seat { get; set; }

        /// <summary>
        /// 游戏数据
        /// </summary>
        public GameData data { get; set; }
    }

    /// <summary>
    /// 游戏数据
    /// </summary>
    [MessagePackObject(true)]
    public class GameData
    {
        /// <summary>
        /// GameID
        /// </summary>
        public ulong id { get; set; }

        /// <summary>
        /// Stage 数据
        /// </summary>
        public StageData sdata { get; set; }
    }

    /// <summary>
    /// Stage 数据
    /// </summary>
    [MessagePackObject(true)]
    public class StageData
    {
        /// <summary>
        /// 游戏的种子，用于随机数生成等目的
        /// </summary>
        public int seed { get; set; }

        /// <summary>
        /// 玩家数据数组，包含了所有参与游戏的玩家信息
        /// </summary>
        public PlayerData[] players { get; set; }
    }

    /// <summary>
    /// 玩家数据
    /// </summary>
    [MessagePackObject(true)]
    public class PlayerData
    {
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; set; }
        
        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// 旋转
        /// </summary>
        public Vector3 euler { get; set; }

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 scale { get; set; }
    }
}
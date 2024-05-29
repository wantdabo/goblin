using MessagePack;
using Queen.Protocols.Common;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Protocols
{
    /// <summary>
    /// 请求退出房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_ExitRoomMsg : INetMessage
    {
    }

    /// <summary>
    /// 请求踢出房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_KickRoomMsg : INetMessage 
    {
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string pid { get; set; }
    }

    /// <summary>
    /// 请求加入房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_JoinRoomMsg : INetMessage
    {
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 房间密码
        /// </summary>
        public uint password { get; set; }
    }

    /// <summary>
    /// 请求房间开局消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_Room2GameMsg : INetMessage
    {

    }

    /// <summary>
    /// 请求销毁房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_DestroyRoomMsg : INetMessage
    {
    }

    /// <summary>
    /// 请求创建房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_CreateRoomMsg : INetMessage
    {
        /// <summary>
        /// 房间名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 需要密码
        /// </summary>
        public bool needpwd { get; set; }
        /// <summary>
        /// 房间密码
        /// </summary>
        public uint password { get; set; }
        /// <summary>
        /// 房间人数
        /// </summary>
        public int mlimit { get; set; }
    }

    /// <summary>
    /// 请求房间列表消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2S_PullRoomsMsg : INetMessage
    {
    }

    /// <summary>
    /// 响应退出房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_ExitRoomMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 退出成功，2 未进入任何房间，3 游戏进行中
        /// </summary>
        public int code { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_KickRoomMsg : INetMessage 
    {
        /// <summary>
        /// 操作码/ 1 该成员已被请离房间，2 您已被请离房间，3 该成员不存在此房间，4 您没有该权限这么做
        /// </summary>
        public int code { get; set; }
    }

    /// <summary>
    /// 请求加入房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_JoinRoomMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 加入成功，2 密码错误，3 房间不存在，4 已有加入的房间，5 房间成员已满
        /// </summary>
        public int code { get; set; }
    }

    /// <summary>
    /// 响应房间开局消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_Room2GameMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 开局成功，2 房间已经在对局中，3 房间不存在，4 无法开启房间
        /// </summary>
        public int code { get; set; }
    }

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
        public uint port { get; set; }
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
    /// 响应销毁房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_DestroyRoomMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 销毁成功，2 没有权限，3 房间不存在
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id { get; set; }
    }

    /// <summary>
    /// 响应创建房间消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_CreateRoomMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 创建成功，2 已有加入的房间
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id { get; set; }
    }

    /// <summary>
    /// 响应房间列表消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_PushRoomsMsg : INetMessage
    {
        /// <summary>
        /// 房间列表
        /// </summary>
        public RoomInfo[] rooms { get; set; }
    }

    /// <summary>
    /// 响应房间列表消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2C_PushRoomMsg : INetMessage
    {
        /// <summary>
        /// 房间
        /// </summary>
        public RoomInfo room { get; set; }
    }

    /// <summary>
    /// 房间信息
    /// </summary>
    [MessagePackObject(true)]
    public class RoomInfo
    {
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 房间状态/ 1 等待加入，2 游戏进行中
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 房主
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// 房间名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 需要密码
        /// </summary>
        public bool needpwd { get; set; }
        /// <summary>
        /// 房间人数
        /// </summary>
        public int mlimit { get; set; }
        /// <summary>
        /// 房间成员
        /// </summary>
        public RoomMemberInfo[] members { get; set; }
    }

    /// <summary>
    /// 房间成员信息
    /// </summary>
    [MessagePackObject(true)]
    public class RoomMemberInfo
    {
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
    }
}

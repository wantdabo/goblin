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
    /// 请求登出消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SLogoutMsg : INetMessage
    {
    }

    /// <summary>
    /// 请求登录消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SLoginMsg : INetMessage
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// 请求注册消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SRegisterMsg : INetMessage
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// 响应登出消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CLogoutMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 登出成功，2 该用户未登录，3 被顶号了
        /// </summary>
        public int code { get; set; }
    }

    /// <summary>
    /// 响应登录消息
    /// </summay>
    [MessagePackObject(true)]
    public class S2CLoginMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 登录成功，2 用户不存在，3 密码错误
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string uuid { get; set; }
    }
    
    /// <summary>
    /// Role 初始化完成消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CRoleJoinedMsg : INetMessage
    {
    }

    /// <summary>
    /// 响应注册消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CRegisterMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 注册成功，2 用户已存在
        /// </summary>
        public int code { get; set; }
    }
}
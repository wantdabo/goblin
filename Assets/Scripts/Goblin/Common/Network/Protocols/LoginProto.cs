using MessagePack;
using Queen.Network.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Network.Protocols
{
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

    [MessagePackObject(true)]
    public class S2CLoginMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 登录成功，2 用户不存在，3 密码错误
        /// </summary>
        public int code { get; set; }
    }

    [MessagePackObject(true)]
    public class S2CRegisterMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 注册成功，2 用户已存在
        /// </summary>
        public int code { get; set; }
    }
}

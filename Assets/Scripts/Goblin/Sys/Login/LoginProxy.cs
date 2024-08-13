using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Goblin.Sys.Login.View;
using Goblin.Sys.Other.View;
using Queen.Protocols;
using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Login
{
    /// <summary>
    /// 登入事件
    /// </summary>
    public struct LoginEvent : IEvent {}

    /// <summary>
    /// 登出事件
    /// </summary>
    public struct LogoutEvent : IEvent { }

    /// <summary>
    /// 登录
    /// </summary>
    public class LoginProxy : Proxy<LoginModel>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Recv<NodeConnectMsg>(OnNodeConnect);
            engine.net.Recv<NodeDisconnectMsg>(OnNodeDisconnect);
            engine.net.Recv<S2CLoginMsg>(OnS2CLogin);
            engine.net.Recv<S2CLogoutMsg>(OnS2CLogout);
            engine.net.Recv<S2CRegisterMsg>(OnS2CRegister);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnRecv<NodeConnectMsg>(OnNodeConnect);
            engine.net.UnRecv<NodeDisconnectMsg>(OnNodeDisconnect);
            engine.net.UnRecv<S2CLoginMsg>(OnS2CLogin);
            engine.net.UnRecv<S2CLogoutMsg>(OnS2CLogout);
            engine.net.UnRecv<S2CRegisterMsg>(OnS2CRegister);
        }

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public void C2SLogin(string username, string password)
        {
            engine.net.Send(new C2SLoginMsg { username = username, password = password });
        }

        /// <summary>
        /// 请求登出
        /// </summary>
        public void C2SLogout()
        {
            engine.net.Send(new C2SLogoutMsg { uuid = data.uuid });
        }

        /// <summary>
        /// 请求注册
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public void C2SRegister(string username, string password)
        {
            engine.net.Send(new C2SRegisterMsg { username = username, password = password });
        }

        private void OnS2CLogin(S2CLoginMsg msg)
        {
            if (1 == msg.code)
            {
                eventor.Tell<LoginEvent>();
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "登录成功." });
                data.uuid = msg.uuid;
                data.signined = true;
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "用户未注册." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "用户密码错误." });
            }
        }

        private void OnS2CLogout(S2CLogoutMsg msg)
        {
            if (1 == msg.code)
            {
                eventor.Tell<LogoutEvent>();
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "用户登出." });
                data.uuid = null;
                data.signined = false;
                engine.gameui.QuickClose();
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "用户未登录." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "此用户已在另一台机器登录." });
                data.uuid = null;
                data.signined = false;
                engine.gameui.QuickClose();
            }
        }

        private void OnS2CRegister(S2CRegisterMsg msg)
        {
            if (1 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "注册成功." });
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "用户已存在." });
            }
        }

        private void OnNodeConnect(NodeConnectMsg msg)
        {
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "连接成功." });
        }

        private void OnNodeDisconnect(NodeDisconnectMsg msg)
        {
            if(data.signined) engine.gameui.QuickClose();
            engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = data.signined ? "强制登出，连接断开." : "连接断开." });
            data.uuid = null;
            data.signined = false;
        }
    }
}

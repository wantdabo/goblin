using Goblin.Sys.Common;
using Goblin.Sys.Lobby;
using Queen.Network.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Login
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginProxy : Proxy
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Listen<S2CLoginMsg>(OnS2CLogin);
            engine.net.Listen<S2CRegisterMsg>(OnS2CRegister);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnListen<S2CLoginMsg>(OnS2CLogin);
            engine.net.UnListen<S2CRegisterMsg>(OnS2CRegister);
        }

        public void C2SLogin(string username, string password)
        {
            engine.net.Send(new C2SLoginMsg { username = username, password = password });
        }

        public void C2SRegister(string username, string password)
        {
            engine.net.Send(new C2SRegisterMsg { username = username, password = password });
        }

        private void OnS2CLogin(S2CLoginMsg msg)
        {
            if (1 == msg.code)
            {
                UnityEngine.Debug.Log("登录成功了");
                engine.gameui.Close<LoginView>();
                engine.gameui.Open<LobbyView>();
            }
            else if (2 == msg.code)
            {
                UnityEngine.Debug.Log("用户未注册");
            }
            else if (3 == msg.code)
            {
                UnityEngine.Debug.Log("密码错误了");
            }
        }

        private void OnS2CRegister(S2CRegisterMsg msg)
        {
            if (1 == msg.code)
            {
                UnityEngine.Debug.Log("注册成功了");
            }
            else if (2 == msg.code)
            {
                UnityEngine.Debug.Log("用户已存在");
            }
        }
    }
}

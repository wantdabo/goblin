using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Login.View
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Login/LoginView";

        private InputField userNameInput;
        private InputField passwordInput;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            userNameInput = engine.u3dkit.SeekNode<InputField>(gameObject, "UserName");
            passwordInput = engine.u3dkit.SeekNode<InputField>(gameObject, "Password");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("RegBtn", (e) =>
            {
                var userName = userNameInput.text;
                var password = passwordInput.text;
                engine.proxy.login.C2SRegister(userName, password);
            });

            AddUIEventListener("LoginBtn", (e) =>
            {
                var userName = userNameInput.text;
                var password = passwordInput.text;
                engine.proxy.login.C2SLogin(userName, password);
            });
        }
    }
}

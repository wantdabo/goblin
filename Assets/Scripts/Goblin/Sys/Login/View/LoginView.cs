using Goblin.Sys.Common;
using Queen.Network.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Login
{
    public class LoginView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Login/LoginView";

        private InputField userNameInputField;
        private InputField passwordInputField;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            userNameInputField = engine.u3dkit.SeekNode<InputField>(gameObject, "UserName");
            passwordInputField = engine.u3dkit.SeekNode<InputField>(gameObject, "Password");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("RegBtn", (e) =>
            {
                var userName = userNameInputField.text;
                var password = passwordInputField.text;
                engine.proxy.login.C2SRegister(userName, password);
            });

            AddUIEventListener("LoginBtn", (e) =>
            {
                var userName = userNameInputField.text;
                var password = passwordInputField.text;
                engine.proxy.login.C2SLogin(userName, password);
            });
        }
    }
}

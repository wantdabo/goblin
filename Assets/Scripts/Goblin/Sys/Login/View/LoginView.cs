using Goblin.Sys.Common;
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
            userNameInputField.text = PlayerPrefs.GetString("userName");
            passwordInputField.text = PlayerPrefs.GetString("password");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("SkipBtn", (e) =>
            {
                engine.gameui.Close<LoginView>();
                //engine.gameui.Open<GameplayView>();
                //engine.gameui.Open<LobbyView>();
            });

            AddUIEventListener("RegBtn", (e) =>
            {
                var userName = userNameInputField.text;
                var password = passwordInputField.text;
                PlayerPrefs.SetString("userName", userName);
                PlayerPrefs.SetString("password", password);
            });

            AddUIEventListener("LoginBtn", (e) =>
            {
                var userName = userNameInputField.text;
                var password = passwordInputField.text;
                PlayerPrefs.SetString("userName", userName);
                PlayerPrefs.SetString("password", password);
            });
        }
    }
}

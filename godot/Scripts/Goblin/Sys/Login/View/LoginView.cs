using Goblin.Sys.Common;
using Godot;

namespace Goblin.Sys.Login.View
{
    public class LoginView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;
        protected override string res => "Login/LoginView";

        private LineEdit userNameInput;
        private LineEdit passwordInput;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            userNameInput = node.FindChild("UserName", true, false) as LineEdit;
            passwordInput = node.FindChild("Password", true, false) as LineEdit;
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("LoginBtn", () =>
            {
                engine.proxy.login.C2SLogin(userNameInput?.Text ?? "", passwordInput?.Text ?? "");
            });
            AddUIEventListener("RegBtn", () =>
            {
                engine.proxy.login.C2SRegister(userNameInput?.Text ?? "", passwordInput?.Text ?? "");
            });
        }
    }
}

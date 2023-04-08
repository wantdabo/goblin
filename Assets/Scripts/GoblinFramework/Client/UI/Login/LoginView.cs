using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Base;
using GoblinFramework.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoblinFramework.Client.UI.Login
{
    public class LoginView : UIBaseView
    {
        public override GameUI.UILayer uilayer => GameUI.UILayer.UIMain;

        protected override string layer => "Login/LoginView";

        protected override void OnBuildUI()
        {
            base.OnBuildUI();

            // 挂载一个 UI 组件
            AddUICell<LoginEnterCell>("LoginEnterContent");
        }
    }
}

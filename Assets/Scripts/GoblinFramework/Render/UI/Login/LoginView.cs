using GoblinFramework.Render.Common;
using GoblinFramework.Render.UI.Base;
using GoblinFramework.Render.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoblinFramework.Render.UI.Login
{
    public class LoginView : UIBaseView
    {
        public override GameUI.UILayer UILayer => GameUI.UILayer.UIMain;

        protected override string UIRes => "Login/LoginView";

        protected override void OnBuildUI()
        {
            base.OnBuildUI();

            // 挂载一个 UI 组件
            AddUICell<LoginEnterCell>("LoginEnterContent");
        }
    }
}

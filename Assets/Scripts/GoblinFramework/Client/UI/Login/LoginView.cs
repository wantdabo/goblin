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
        public override GameUI.UILayer UILayer => GameUI.UILayer.UIMain;

        protected override string UIRes => "Login/LoginView";

        private GameObject Hoshi;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();

            Hoshi = Engine.GameRes.Location.LoadActorPrefabSync("Hoshi/Hoshi");

            // 挂载一个 UI 组件
            AddUICell<LoginEnterCell>("LoginEnterContent");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameObject.Destroy(Hoshi);
        }
    }
}

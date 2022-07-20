using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Base;
using GoblinFramework.Client.UI.Common;
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
        protected override string UIRes => "Login/LoginView";

        public override GameUIComp.UILayer UILayer => GameUIComp.UILayer.UIMain;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            Engine.U3D.SeekNode<Text>(go, "Title").text = "Goblin Framework";

            int counter = 0;
            var clock = AddComp<ClockComp>();
            var textClock = Engine.U3D.SeekNode<Text>(go, "ClockText");
            clock.Start(() =>
            {
                counter++;
                textClock.text = counter.ToString();
            }, 0.0001f, -1);
        }
    }
}

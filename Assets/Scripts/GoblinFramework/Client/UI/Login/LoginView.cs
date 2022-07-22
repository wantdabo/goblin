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
        public override GameUIComp.UILayer UILayer => GameUIComp.UILayer.UIMain;

        protected override string UIRes => "Login/LoginView";

        protected override void OnBuildUI()
        {
            base.OnBuildUI();

            //// 测试挂载速度
            //for (int i = 0; i < 1000; i++) { 
            //    var cell = AddUICell<LoginEnterCell>("LoginEnterContent", false);
            //}

            // 挂载一个 UI 组件
            AddUICell<LoginEnterCell>("LoginEnterContent");

            // 测试 API，模糊查找组件
            Engine.U3D.SeekNode<Text>(gameObject, "Title").text = "Goblin Framework";

            // 测试定时器
            int counter = 0;
            var clock = AddComp<ClockComp>();
            var textClock = Engine.U3D.SeekNode<Text>(gameObject, "ClockText");
            clock.Start(() =>
            {
                counter++;
                textClock.text = counter.ToString();
            }, 0.0001f, -1);
        }
    }
}

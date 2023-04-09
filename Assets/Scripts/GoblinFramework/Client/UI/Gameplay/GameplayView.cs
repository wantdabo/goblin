using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.Gameplay.Events;
using UnityEngine;
using UnityEngine.UI;

namespace GoblinFramework.Client.UI.Gameplay
{
    public class GameplayView : UIBaseView, IUpdate
    {
        public override GameUI.UILayer layer => GameUI.UILayer.UIMain;

        protected override string res => "Gameplay/GameplayView";

        private Text clockText;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            clockText = engine.u3d.SeekNode<Text>(gameObject, "ClockText");
        }

        private TestGameStage stage;
        protected override void OnOpen()
        {
            base.OnOpen();
            stage = TestGameStage.CreateGameStage(null);
            stage.eventor.Listen<TestEvent>(TestEventFunc);
        }

        private void TestEventFunc(TestEvent evt)
        {
            Debug.Log(evt.testStr);
            // stage.eventor.UnListen<TestEvent>(TestEventFunc);
        }

        public void Update(float tick)
        {
            if (null == clockText) return;
            if (null == stage) return;
            
            clockText.text = DateTime.Now.ToLongTimeString();
            
            var e = new TestEvent();
            e.testStr = "Hello World.";
            stage.eventor.Tell(e);
        }
    }
}

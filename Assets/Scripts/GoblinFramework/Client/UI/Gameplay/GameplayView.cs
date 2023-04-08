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
        public override GameUI.UILayer uilayer => GameUI.UILayer.UIMain;

        protected override string layer => "Gameplay/GameplayView";

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
            stage.eventor.Hear<TestEvent>((e) =>
            {
                Debug.Log(e.testStr);
            });
        }
        
        public void Update(float tick)
        {
            if (null == clockText) return;
            if (null == stage) return;
            
            clockText.text = DateTime.Now.ToLongTimeString();
            
            var e = new TestEvent();
            e.testStr = "Hello World.";
            stage.eventor.Tell<TestEvent>(e);
        }
    }
}

using GoblinFramework.Render.Common;
using GoblinFramework.Render.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.Common.Events;
using UnityEngine;
using UnityEngine.UI;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Render.UI.Gameplay
{
    public class GameplayView : UIBaseView
    {
        public override GameUI.UILayer layer => GameUI.UILayer.UIMain;

        protected override string res => "Gameplay/GameplayView";

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<UpdateEvent>(OnUpdate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<UpdateEvent>(OnUpdate);
        }

        private Text clockText;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            clockText = engine.u3dtool.SeekNode<Text>(gameObject, "ClockText");
        }

        private TestGameStage stage;
        protected override void OnOpen()
        {
            base.OnOpen();
            stage = TestGameStage.CreateGameStage(null);
            stage.Analyze(null);
            stage.Play();
            stage.Pause();
            stage.Play();
            
            var jianhun = stage.AddActor<Jianhun>();
            jianhun.Create();
        }

        public void OnUpdate(UpdateEvent e)
        {
            if (null == clockText) return;
            if (null == stage) return;
            stage.Tick(e.tick);
            clockText.text = DateTime.Now.ToLongTimeString();
        }
    }
}

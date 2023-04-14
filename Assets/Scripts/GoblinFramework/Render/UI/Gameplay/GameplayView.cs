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
using GoblinFramework.Render.Gameplay;

namespace GoblinFramework.Render.UI.Gameplay
{
    public class GameplayView : UIBaseView, IUpdate
    {
        public override GameUI.UILayer layer => GameUI.UILayer.UIMain;

        protected override string res => "Gameplay/GameplayView";

        private Text clockText;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            clockText = engine.u3dtool.SeekNode<Text>(gameObject, "ClockText");
        }

        private TestGameStage stage;
        private SurfaceDirector director;
        protected override void OnOpen()
        {
            base.OnOpen();
            stage = TestGameStage.CreateGameStage(null);
            director = AddComp<SurfaceDirector>();
            director.Create(stage);
            stage.Analyze(null);
            stage.Play();
            stage.Pause();
            stage.Play();
            var actor = stage.AddActor<Actor>();
            actor.Create();
            stage.RmvActor(actor);
        }

        public void Update(float tick)
        {
            if (null == clockText) return;
            if (null == stage) return;
            
            clockText.text = DateTime.Now.ToLongTimeString();
        }
    }
}

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

namespace GoblinFramework.Render.UI.GameInitialize
{
    public class GameInitializeView : UIBaseView, IUpdate
    {
        public override GameUI.UILayer layer => GameUI.UILayer.UITop;

        protected override string res => "GameInitialize/GameInitializeView";

        private Slider sliderProgress;
        private Text textProgress;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            sliderProgress = engine.u3dtool.SeekNode<Slider>(gameObject, "Progress");
            textProgress = engine.u3dtool.SeekNode<Text>(gameObject, "ProgressDesctText");
        }

        private float speed = 1f;
        public float progress = 0;
        public void Update(float tick)
        {
            if (null == gameObject) return;

            if (progress >= 1) return;
            
            progress += tick * speed;

            Mathf.Clamp(progress, 0, 1);
            sliderProgress.value = progress;

            if (progress >= 1)
            {
                engine.gameui.Open<Login.LoginView>();
                engine.gameui.Close<GameInitializeView>();
                return;
            }

            if (progress >= 0.6f)
                textProgress.text = "正在初始化游戏...";
            
            if (progress >= 0.3f)
                textProgress.text = "初始化配置...";
            
            if (progress >= 0f)
                textProgress.text = "检查更新...";
        }
    }
}

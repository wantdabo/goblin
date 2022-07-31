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

namespace GoblinFramework.Client.UI.GameInitialize
{
    public class GameInitializeView : UIBaseView, IUpdate
    {
        public override GameUI.UILayer UILayer => GameUI.UILayer.UITop;

        protected override string UIRes => "GameInitialize/GameInitializeView";

        private Slider sliderProgress;
        private Text textProgress;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            sliderProgress = Engine.U3D.SeekNode<Slider>(gameObject, "Progress");
            textProgress = Engine.U3D.SeekNode<Text>(gameObject, "ProgressDesctText");
        }

        private float speed = 1f;
        public float progress = 0;
        public void Update(float tick)
        {
            if (null == gameObject) return;

            if (progress >= 1) return;

            sliderProgress.value = progress;

            progress += tick * speed;

            Mathf.Clamp(progress, 0, 1);

            if (progress <= 0.3f)
                textProgress.text = "检查更新...";
            else if (progress >= 0.3f && progress <= 0.6f)
                textProgress.text = "初始化配置...";
            else if (progress >= 0.6f)
                textProgress.text = "正在初始化游戏...";
        }
    }
}

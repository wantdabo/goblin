﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Goblin.Common;
using Goblin.Core;
using Goblin.Sys.Common;

namespace Goblin.Sys.GameInitialize
{
    public class GameInitializeView : UIBaseView
    {
        public override UILayer layer => UILayer.UITop;

        protected override string res => "GameInitialize/GameInitializeView";

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        private Slider sliderProgress;
        private Text textProgress;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            sliderProgress = engine.u3dkit.SeekNode<Slider>(gameObject, "Progress");
            textProgress = engine.u3dkit.SeekNode<Text>(gameObject, "ProgressDesctText");
        }

        private float speed = 1f;
        public float progress = 0;
        public void OnTick(TickEvent e)
        {
            if (null == gameObject) return;

            if (progress >= 1) return;
            
            progress += e.tick * speed;

            progress = Mathf.Clamp(progress, 0, 1);
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
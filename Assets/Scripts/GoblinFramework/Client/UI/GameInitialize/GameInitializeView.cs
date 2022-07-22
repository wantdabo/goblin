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

namespace GoblinFramework.Client.UI.GameInitialize
{
    public class GameInitializeView : UIBaseView, IUpdate
    {
        public override GameUIComp.UILayer UILayer => GameUIComp.UILayer.UITop;

        protected override string UIRes => "GameInitialize/GameInitializeView";

        private float speed = 0.5f;
        public float progress = 0;
        public void Update(float tick)
        {
            if (null == gameObject) return;

            if (progress >= 1) return;

            Engine.U3D.SeekNode<Slider>(gameObject, "Progress").value = progress;

            progress += tick * speed;

            Mathf.Clamp(progress, 0, 1);
        }
    }
}

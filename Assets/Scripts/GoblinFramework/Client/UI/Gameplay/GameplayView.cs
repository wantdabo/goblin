using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace GoblinFramework.Client.UI.Gameplay
{
    public class GameplayView : UIBaseView, IUpdate
    {
        public override GameUI.UILayer UILayer => GameUI.UILayer.UIMain;

        protected override string UIRes => "Gameplay/GameplayView";

        private Text clockText;
        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            clockText = Engine.U3D.SeekNode<Text>(gameObject, "ClockText");
        }

        public void Update(float tick)
        {
            clockText.text = DateTime.Now.ToLongTimeString();
        }
    }
}

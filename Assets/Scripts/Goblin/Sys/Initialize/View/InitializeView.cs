using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Goblin.Sys.Initialize.View
{
    public class InitializeView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Initialize/InitializeView";

        private Text descText;
        private Slider proSlider;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            descText = engine.u3dkit.SeekNode<Text>(gameObject, "Desc");
            proSlider = engine.u3dkit.SeekNode<Slider>(gameObject, "Pro");
        }

        public void UpdateInfo(string desc, float pro) 
        {
            descText.text = desc;
            proSlider.value = pro;
        }
    }
}

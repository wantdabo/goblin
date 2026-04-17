using Goblin.Sys.Common;
using Godot;

namespace Goblin.Sys.Initialize.View
{
    public class InitializeView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;
        protected override string res => "Initialize/InitializeView";

        private Label descText;
        private ProgressBar proSlider;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            descText = node.FindChild("Desc", true, false) as Label;
            proSlider = node.FindChild("Pro", true, false) as ProgressBar;
        }

        public void UpdateInfo(string desc, float pro)
        {
            if (descText != null) descText.Text = desc;
            if (proSlider != null) proSlider.Value = pro;
        }
    }
}

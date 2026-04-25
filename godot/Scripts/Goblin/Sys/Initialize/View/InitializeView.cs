using Goblin.Sys.Common;
using Godot;

namespace Goblin.Sys.Initialize.View;

public class InitializeView : UIBaseView
{
    public override UILayer layer => UILayer.UIMain;
    protected override string res => "Initialize/InitializeView";

    private Label descText { get; set; }
    private ProgressBar proSlider { get; set; }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        descText = engine.gdkit.SeekNode<Label>(node, "Desc");
        proSlider = engine.gdkit.SeekNode<ProgressBar>(node, "Pro");
    }

    public void UpdateInfo(string desc, float pro)
    {
        if (descText != null) descText.Text = desc;
        if (proSlider != null) proSlider.Value = pro;
    }
}
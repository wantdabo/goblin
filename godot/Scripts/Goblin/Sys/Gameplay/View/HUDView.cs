using Goblin.Common;
using Goblin.Sys.Common;
using Godot;
using System.Collections.Generic;

namespace Goblin.Sys.Gameplay.View;

public class HUDView : UIBaseView
{
    public override UILayer layer => UILayer.UIMain;
    protected override string res => "Gameplay/HUDView";

    private Control contentNode { get; set; }
    private Control barOrgNode { get; set; }
    private List<(Control bar, ColorRect fill)> barpool { get; set; } = new();

    protected override void OnLoad()
    {
        base.OnLoad();
        engine.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        engine.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
    }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        contentNode = engine.gdkit.SeekNode<Control>(node, "Content");
        barOrgNode = engine.gdkit.SeekNode<Control>(node, "BarOrg");
    }

    private void OnLateTick(LateTickEvent e)
    {
        // Phase 2+：从 canvas.GetShadow<T> 读取 Actor 血量信息绘制血条
        // Phase 1：占位
        if (null == engine.proxy.gameplay.stage || contentNode == null || barOrgNode == null) return;

        for (int i = 0; i < barpool.Count; i++) barpool[i].bar.Visible = false;
    }
}

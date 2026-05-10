using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
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
        var local = engine.proxy.gameplay.director as LocalDirector;
        if (null == local || contentNode == null || barOrgNode == null) return;

        var rilactors = local.world.rilbucket.GetRIL<RIL_ACTOR>(local.world.sa);
        var cam = local.world.eyes?.camera;
        if (null == rilactors || null == cam) return;

        int index = 0;
        foreach (var actor in rilactors.actors)
        {
            var rilattr = local.world.rilbucket.GetRIL<RIL_ATTRIBUTE>(actor);
            if (null == rilattr || rilattr.maxhp <= 0) continue;

            var spatialagent = local.world.GetAgent<SpatialAgent>(actor);
            var rilspatial = local.world.rilbucket.GetRIL<RIL_SPATIAL>(actor);
            var worldPos = spatialagent != null
                ? spatialagent.position + new Vector3(0, 2.2f, 0)
                : (rilspatial != null ? rilspatial.position.ToVector3() + new Vector3(0, 2.2f, 0) : Vector3.Zero);

            if (cam.IsPositionBehind(worldPos)) continue;
            var screenPos = cam.UnprojectPosition(worldPos);
            var vp = screenPos / cam.GetViewport().GetVisibleRect().Size;
            if (vp.X < 0 || vp.X > 1 || vp.Y < 0 || vp.Y > 1) continue;

            if (index >= barpool.Count)
            {
                var newBar = (Control)barOrgNode.Duplicate();
                contentNode.AddChild(newBar);
                var fill = engine.gdkit.SeekNode<ColorRect>(newBar, "Fill");
                barpool.Add((newBar, fill));
            }

            var (bar, fillRect) = barpool[index];
            bar.Visible = true;

            float ratio = Mathf.Clamp((float)rilattr.hp / rilattr.maxhp, 0f, 1f);
            float barW = bar.Size.X;
            if (fillRect != null) fillRect.Size = new Vector2(barW * ratio, fillRect.Size.Y);

            // 英雄绿色，敌人红色
            bool isHero = actor == local.world.self;
            if (fillRect != null) fillRect.Color = isHero
                ? new Color(0.2f, 0.85f, 0.2f, 1f)
                : new Color(0.85f, 0.2f, 0.2f, 1f);

            bar.Position = screenPos - new Vector2(barW * 0.5f, 0);
            index++;
        }

        for (int i = index; i < barpool.Count; i++) barpool[i].bar.Visible = false;
    }
}

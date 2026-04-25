using Goblin.Common;
using Goblin.Sys.Common;
using Godot;

namespace Goblin.Sys.Gameplay.View;

public class GameplayDanceView : UIBaseView
{
    protected override string res => "Gameplay/GameplayDanceView";
    public override UILayer layer => UILayer.UIAlert;

    private Control contentNode { get; set; }
    private Control damageOrgNode { get; set; }

    protected override void OnLoad()
    {
        base.OnLoad();
        engine.proxy.gameplay.eventor.Listen<CureDanceEvent>(OnCureDance);
        engine.proxy.gameplay.eventor.Listen<DamageDanceEvent>(OnDamageDance);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        engine.proxy.gameplay.eventor.UnListen<CureDanceEvent>(OnCureDance);
        engine.proxy.gameplay.eventor.UnListen<DamageDanceEvent>(OnDamageDance);
    }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        contentNode = engine.gdkit.SeekNode<Control>(node, "Content");
        damageOrgNode = engine.gdkit.SeekNode<Control>(node, "DamageOrg");
    }

    private void OnCureDance(CureDanceEvent e)
    {
        if (!engine.proxy.gameplay.dancing) return;
    }

    private void OnDamageDance(DamageDanceEvent e)
    {
        if (!engine.proxy.gameplay.dancing) return;
        if (damageOrgNode == null || contentNode == null) return;

        var obj = ObjectPool.Get<GameplayDanceObject>("BLOOD_DANCE_DAMAGE");
        if (null == obj)
        {
            var go = (Control)damageOrgNode.Duplicate();
            contentNode.AddChild(go);
            obj = new GameplayDanceObject(go);
        }
        obj.node.Position = e.screenpos;
        obj.Settings(e.damage);
        obj.node.Visible = true;
        obj.Play();
        engine.ticker.Timing((t) =>
        {
            obj.node.Visible = false;
            ObjectPool.Set(obj, "BLOOD_DANCE_DAMAGE");
        }, 0.7f, 1);
    }
}
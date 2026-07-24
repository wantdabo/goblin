using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Sys.Common;
using Godot;

namespace Goblin.Gameplay.Render.Resolvers.Salutes;

/// <summary>
/// 伤害事件处理器
/// </summary>
public class DamageSalute : RILSalute<RIL_EVENT_DAMAGE>
{
    protected override void OnSalute(RIL_EVENT_DAMAGE e)
    {
        var position = Vector3.Up * 0.9f;
        var node = rilbucket.world.GetAgent<SpatialAgent>(e.to);
        if (node != null) position += node.position;

        Vector2 screenpos = Vector2.Zero;
        var cam = rilbucket.world.eyes?.camera;
        if (null != cam) screenpos = cam.UnprojectPosition(position);

        engine.proxy.gameplay.eventor.Tell(new DamageDanceEvent
        {
            screenpos = screenpos,
            crit = e.crit,
            damage = e.damage,
            from = e.from,
            to = e.to
        });
    }
}
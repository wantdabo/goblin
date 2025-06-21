using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Sys.Gameplay.View;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers.Salutes
{
    /// <summary>
    /// 伤害事件处理器
    /// </summary>
    public class DamageSalute : RILSalute<RIL_EVENT_DAMAGE>
    {
        protected override void OnSalute(RIL_EVENT_DAMAGE e)
        {
            // TODO 后续应该改为读跳字挂点位置
            Vector3 position = Vector3.up * (1.8f / 2f);
            var node = rilbucket.world.GetAgent<NodeAgent>(e.to);
            if (null != node) position += node.go.transform.position;
            
            engine.proxy.gameplay.eventor.Tell(new DamageDanceEvent
            {
                screenpos = rilbucket.world.eyes.camera.WorldToScreenPoint(position),
                crit = e.crit,
                damage = e.damage,
                from = e.from,
                to = e.to
            });
        }
    }
}
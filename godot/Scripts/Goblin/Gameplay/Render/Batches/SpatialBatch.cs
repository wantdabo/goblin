using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;

namespace Goblin.Gameplay.Render.Batches
{
    /// <summary>
    /// 空间批处理，替代 Unity Jobs 版本，使用普通 C# 循环 + Godot 插值
    /// </summary>
    public class SpatialBatch : Batch
    {
        protected override void OnTick(TickEvent e)
        {
            base.OnTick(e);
            if (false == world.rilbucket.SeekRILS<RIL_SPATIAL>(out var rils)) return;

            float t = Mathf.Clamp(e.tick, 0, GAME_DEFINE.MAX_TICK) / GAME_DEFINE.LOGIC_TICK.AsFloat();
            t = Mathf.Clamp(t, 0f, 1f);

            foreach (var ril in rils)
            {
                var nodeAgent = world.GetAgent<NodeAgent>(ril.actor);
                if (null == nodeAgent || ChaseStatus.Arrived == nodeAgent.status) continue;

                var timescale = 1f;
                if (world.rilbucket.SeekRIL<RIL_TICKER>(ril.actor, out var ticker))
                    timescale = Mathf.Clamp(ticker.timescale * Config.Int2Float, 0f, 1f);

                var node = nodeAgent.node;
                var tarpos = ril.position.ToVector3();
                var tarrot = ril.euler.ToVector3() * MathF.PI / 180f;
                var tarscale = Vector3.One * ril.scale.AsFloat();

                node.Position = node.Position.Lerp(tarpos, t * timescale);
                node.Rotation = node.Rotation.Lerp(tarrot, t * timescale);
                node.Scale = tarscale;
            }

            rils.Clear();
            ObjectPool.Set(rils);
        }
    }
}

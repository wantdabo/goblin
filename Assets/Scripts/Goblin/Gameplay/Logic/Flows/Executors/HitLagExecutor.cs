using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 顿帧执行器
    /// </summary>
    public class HitLagExecutor : Executor<HitLagData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, HitLagData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo collisionhurt)) return;
            var targetcnt = collisionhurt.targets.Count;
            if (0 == targetcnt) return;

            var strength = data.strength * FP.EN3;
            var duration = data.duration * FP.EN3;
            if (HIT_LAG_DEFINE.TYPE_ADDITIVE == data.type)
            {
                if (targetcnt > 1)
                {
                    var factor = (targetcnt - 1) * data.additivefactor * FP.EN3;
                    strength += strength * factor;
                    duration += duration * factor;
                    strength = FPMath.Clamp(strength, 0, data.strengthmax * FP.EN3);
                    duration = FPMath.Clamp(duration, 0, data.durationmax * FP.EN3);
                }
            }

            if (stage.SeekBehaviorInfo(target, out HitLagInfo hitlag))
            {
                // 已存在顿帧信息，取更大值覆盖
                if (hitlag.strength < strength)
                {
                    hitlag.strength = strength;
                }
                if (hitlag.duration < duration)
                {
                    hitlag.duration = duration;
                }
                return;
            }

            stage.hiteffect.AddHitLag(target, strength, duration);
        }
    }
}
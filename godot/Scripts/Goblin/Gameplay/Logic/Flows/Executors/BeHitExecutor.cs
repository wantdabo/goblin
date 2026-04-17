using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 受击执行器
    /// </summary>
    public class BeHitExecutor : Executor<BeHitData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, BeHitData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (stage.SeekBehaviorInfo(target, out StateMachineInfo statemachine) && STATE_DEFINE.DEATH == statemachine.current) return;
            stage.SeekBehaviorInfo(target, out SpatialInfo spatial);
            stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo atkspatial);
            
            if (data.uselookatattacker && null != spatial && null != atkspatial)
            {
                var dire = (atkspatial.position - spatial.position).normalized;
                FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
                spatial.euler = FPVector3.up * angle;
            }

            if (data.usehitmotion)
            {
                switch (data.hitmotiontype)
                {
                    case BEHIT_DEFINE.MOTION_SELF_FORWARD:
                        if (null != spatial)
                        {
                            var rotation = FPQuaternion.Euler(spatial.euler);
                            spatial.position += rotation * data.hitmotion.ToFPVector3();
                        }
                        break;
                    case BEHIT_DEFINE.MOTION_ATTACK_FORWARD:
                        if (null != spatial && null != atkspatial)
                        {
                            var rotation = FPQuaternion.Euler(atkspatial.euler);
                            spatial.position += rotation * data.hitmotion.ToFPVector3();
                        }
                        break;
                    case BEHIT_DEFINE.MOTION_ATTACKER_TO_SELF:
                        if (null != spatial && null != atkspatial)
                        {
                            var rotation = FPQuaternion.LookRotation((spatial.position - atkspatial.position).normalized, FPVector3.up);
                            spatial.position += rotation * data.hitmotion.ToFPVector3();
                        }
                        break;
                    case BEHIT_DEFINE.MOTION_SELF_TO_ATTACKER:
                        if (null != spatial && null != atkspatial)
                        {
                            var rotation = FPQuaternion.LookRotation((atkspatial.position - spatial.position).normalized, FPVector3.up);
                            spatial.position += rotation * data.hitmotion.ToFPVector3();
                        }
                        break;
                }
            }
        }
    }
}
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 子弹运动指令执行器
    /// </summary>
    public class BulletMotionExecutor : Executor<BulletMotionData>
    {
        protected override void OnExecute(BulletMotionData data, FlowInfo flowinfo)
        {
            base.OnExecute(data, flowinfo);
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out BulletInfo bullet)) return;

            switch (data.motion)
            {
                case FLOW_BULLET_DEFINE.MOTION_STRAIGHT:
                    var rotation = FPQuaternion.Euler(spatial.euler);
                    var forward = rotation * FPVector3.forward;
                    spatial.position += forward * data.speedrate * stage.cfg.int2fp * bullet.speed * GAME_DEFINE.LOGIC_TICK;
                    break;
            }
        }
    }
}
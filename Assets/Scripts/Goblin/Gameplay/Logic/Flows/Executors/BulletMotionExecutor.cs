using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos;
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
                    spatial.position += forward * bullet.speed * GAME_DEFINE.LOGIC_TICK;
                    break;
            }

            // TODO 移除模型测试, 记得删除
            if (flowinfo.timeline >= 3000)
            {
                if (stage.SeekBehavior(flowinfo.owner, out Facade facade))
                {
                    return;
                }
                
                // TODO 临时加模型, 记得删除
                facade = stage.AddBehavior<Facade>(flowinfo.owner);
                facade.SetModel(200001);
            }
            else if (flowinfo.timeline >= 2000)
            {
                if (false == stage.SeekBehavior(flowinfo.owner, out Facade facade)) return;
                stage.RmvBehavior(facade);
            }
        }
    }
}
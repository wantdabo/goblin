using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 测试指令执行器
    /// </summary>
    public class TestExecutor : Executor<TestInstr>
    {
        protected override void OnEnter(TestInstr data, FlowInfo flowinfo)
        {
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            
            stage.Spawn(new HeroPrefabInfo
            {
                hero = 100001,
                spatial = new()
                {
                    position = spatial.position + FPVector3.forward,
                    euler = spatial.euler,
                    scale = spatial.scale,
                }
            });
        }

        protected override void OnExecute(TestInstr data, FlowInfo flowinfo)
        {
        }

        protected override void OnExit(TestInstr data, FlowInfo flowinfo)
        {
        }
    }
}
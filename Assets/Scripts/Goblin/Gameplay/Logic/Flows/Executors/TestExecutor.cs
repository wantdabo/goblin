using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
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
            
            var hero = stage.Spawn(new HeroPrefabInfo
            {
                hero = data.hero,
                spatial = new()
                {
                    position = spatial.position,
                    euler = spatial.euler,
                    scale = spatial.scale,
                }
            });
            hero.AddBehavior<TestAIMove>();
        }

        protected override void OnExecute(TestInstr data, FlowInfo flowinfo)
        {
        }

        protected override void OnExit(TestInstr data, FlowInfo flowinfo)
        {
        }
    }
}
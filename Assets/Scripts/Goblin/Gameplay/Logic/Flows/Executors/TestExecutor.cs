using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Flows.Executors.Common;
using Goblin.Gameplay.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Flows.Executors
{
    /// <summary>
    /// 测试指令执行器
    /// </summary>
    public class TestExecutor : Executor<TestInstr>
    {
        protected override void OnEnter(TestInstr data, FlowInfo flowinfo)
        {
        }

        protected override void OnExecute(TestInstr data, FlowInfo flowinfo)
        {
        }

        protected override void OnExit(TestInstr data, FlowInfo flowinfo)
        {
        }
    }
}
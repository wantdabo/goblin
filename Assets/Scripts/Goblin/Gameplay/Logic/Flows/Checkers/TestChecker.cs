using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;

namespace Goblin.Gameplay.Logic.Flows.Checkers
{
    /// <summary>
    /// 测试指令条件检查器
    /// </summary>
    public class TestChecker : Checker<TestCondi>
    {
        protected override bool OnCheck(TestCondi condition, FlowInfo flowinfo)
        {
            return true;
        }
    }
}
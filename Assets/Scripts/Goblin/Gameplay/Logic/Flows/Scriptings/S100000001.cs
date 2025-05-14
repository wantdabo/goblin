using Goblin.Gameplay.Flows.Checkers.Common;
using Goblin.Gameplay.Flows.Checkers.Conditions;
using Goblin.Gameplay.Flows.Executors.Instructs;
using Goblin.Gameplay.Flows.Scriptings.Common;
using Goblin.Gameplay.Logic.Common.Defines;

namespace Goblin.Gameplay.Flows.Scriptings
{
    public class S100000001 : Scripting
    {
        public override uint id => FLOW_DEFINE.S100000001;

        protected override void OnScript()
        {
            Instruct(0, 1000, new TestInstr { })
                .Condition(new TestCondi { })
                .After(0, 1000, new TestInstr { })
                    .Condition(new TestCondi { })
                    .Condition(new TestCondi { });
        }
    }
}
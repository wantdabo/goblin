using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings
{
    public class S100000001 : Scripting
    {
        public override uint id => FLOW_DEFINE.S100000001;

        protected override void OnScript()
        {
            Instruct(0, 1000, new TestInstr { hero = 200001});
            // Instruct(0, 1000, new TestInstr { content = "TestInstr 1"})
            //     .Condition(new TestCondi { })
            //     .After(0, 1000, new TestInstr { content = "TestInstr 2"})
            //         .Condition(new TestCondi { })
            //         .Condition(new TestCondi { });
        }
    }
}
using Goblin.Gameplay.Flows.Scriptings.Common;
using Goblin.Gameplay.Logic.Flows.Common;

namespace Goblin.Gameplay.Flows.Scriptings
{
    public class TestInstructData : InstructData
    {
        public override ushort id => 1;
    }
    
    public class TestCondition : Condition
    {
        public override ushort id => 1;
    }

    public class S100000001 : Scripting
    {
        public override uint id => 100000001;
        
        protected override void OnScript()
        {
            Instruct(0, 1000, new TestInstructData())
            .Condition(new TestCondition())
            .After(0, 1000, new TestInstructData())
            .Condition(new TestCondition())
            .Condition(new TestCondition());
        }
    }
}
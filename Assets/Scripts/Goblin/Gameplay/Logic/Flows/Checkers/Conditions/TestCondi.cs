using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Conditions
{
    public class TestCondi : Condition
    {
        public override ushort id => CONDITION_DEFINE.TEST;
        
        public uint timescale { get; set; }
    }
}
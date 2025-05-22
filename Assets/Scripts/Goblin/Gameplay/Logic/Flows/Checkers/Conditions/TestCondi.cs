using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Conditions
{
    public class TestCondi : Condition
    {
        public override ushort id => 1;
        
        public uint timescale { get; set; }
    }
}
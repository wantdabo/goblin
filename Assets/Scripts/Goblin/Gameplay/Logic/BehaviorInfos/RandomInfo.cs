using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class RandomInfo : IBehaviorInfo
    {
        public long a { get; private set; }
        public long c { get; private set; }
        public long m { get; private set; }
        public long seed { get; set; }
        public long current { get; set; }

        public void Ready()
        {
            Reset();
        }

        public void Reset()
        {
            a =  1664525;
            c = 1013904223;
            m = int.MaxValue;
            seed = 0;
            current = 0;
        }
    }
}
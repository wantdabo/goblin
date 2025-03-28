using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class RandomInfo : IBehaviorInfo
    {
        public long a { get; private set; } = 1664525;
        public long c { get; private set; } = 1013904223;
        public long m { get; private set; } = int.MaxValue;
        public long seed { get; set; }
        public long current { get; set; }

        public void Ready()
        {
            Reset();
        }

        public void Reset()
        {
            seed = 0;
            current = 0;
        }
    }
}
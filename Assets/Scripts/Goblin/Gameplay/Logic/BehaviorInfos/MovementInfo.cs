using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class MovementInfo : IBehaviorInfo
    {
        public bool motion { get; set; }
        
        public void Ready()
        {
            Reset();
        }

        public void Reset()
        {
            motion = false;
        }
    }
}
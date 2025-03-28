using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class MovementInfo : IBehaviorInfo
    {
        public void OnReady()
        {
            OnReset();
        }

        public void OnReset()
        {
        }
    }
}
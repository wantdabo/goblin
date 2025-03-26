using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class MovementInfo : BehaviorInfo
    {
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
        }
    }
}
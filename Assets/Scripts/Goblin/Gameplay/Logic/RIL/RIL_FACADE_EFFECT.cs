using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    public class RIL_FACADE_EFFECT : IRIL
    {
        public override ushort id => RIL_DEFINE.FACADE_EFFECT;
        
        protected override void OnReady()
        {
        }

        protected override void OnReset()
        {
        }
    }
}
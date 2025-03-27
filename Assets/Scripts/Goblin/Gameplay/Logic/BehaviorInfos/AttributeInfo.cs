using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class AttributeInfo : BehaviorInfo
    {
        public uint hp { get; set; }
        public uint maxhp { get; set; }
        public uint moveseed { get; set; }
        public uint attack { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            hp = 0;
            maxhp = 0;
            moveseed = 0;
            attack = 0;
        }
    }
}
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class AttributeInfo : IBehaviorInfo
    {
        public uint hp { get; set; }
        public uint maxhp { get; set; }
        public uint moveseed { get; set; }
        public uint attack { get; set; }
        
        public void Ready()
        {
            Reset();
        }

        public void Reset()
        {
            hp = 0;
            maxhp = 0;
            moveseed = 0;
            attack = 0;
        }
    }
}
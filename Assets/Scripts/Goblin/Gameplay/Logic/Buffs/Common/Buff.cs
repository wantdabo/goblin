using Goblin.Core;

namespace Goblin.Gameplay.Logic.Buffs.Common
{
    public abstract class Buff : Comp
    {
        public abstract uint id { get; }
        public uint layer { get; set; }
        public uint maxlayer { get; set; }
        public BuffBucket bucket { get; set; }
    }
}

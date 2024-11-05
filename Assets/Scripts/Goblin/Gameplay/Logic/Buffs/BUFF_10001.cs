using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Buffs.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Buffs
{
    /// <summary>
    /// 感电 BUFF
    /// </summary>
    public class BUFF_10001 : Buff
    {
        public override uint id => BUFF_DEFINE.BUFF_10001;
        
        public override byte type => BUFF_DEFINE.SHARED;
        
        public override uint maxlayer => 1;

        protected override bool OnValid()
        {
            return layer >= 1;
        }
    }
}

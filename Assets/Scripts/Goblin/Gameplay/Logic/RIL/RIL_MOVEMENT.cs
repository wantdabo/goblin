using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 运动指令
    /// </summary>
    public class RIL_MOVEMENT : IRIL
    {
        public override ushort id => RIL_DEFINE.MOVEMENT;
        
        /// <summary>
        /// 运动类型
        /// </summary>
        public byte motion { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            motion = 0;
        }
    }
}
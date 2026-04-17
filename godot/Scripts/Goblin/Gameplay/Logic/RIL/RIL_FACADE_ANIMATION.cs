using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 外观动画指令
    /// </summary>
    public class RIL_FACADE_ANIMATION : IRIL
    {
        public override ushort id => RIL_DEFINE.FACADE_ANIMATION;
        
        /// <summary>
        /// 动画状态
        /// </summary>
        public byte animstate { get; set; }
        /// <summary>
        /// 动画名称
        /// </summary>
        public string animname { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public uint animelapsed { get; set; }
        
        protected override void OnReady()
        {
            animstate = 0;
            animname = null;
            animelapsed = 0;
        }

        protected override void OnReset()
        {
            animstate = 0;
            animname = null;
            animelapsed = 0;
        }
    }
}
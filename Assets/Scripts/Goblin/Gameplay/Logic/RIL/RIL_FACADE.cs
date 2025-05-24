using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 外观指令
    /// </summary>
    public class RIL_FACADE : IRIL
    {
        public override ushort id => RIL_DEFINE.FACADE;
        
        /// <summary>
        /// 模型 ID
        /// </summary>
        public int model { get; set; }
        
        protected override void OnReady()
        {
            model = 0;
        }

        protected override void OnReset()
        {
            model = 0;
        }
    }
}
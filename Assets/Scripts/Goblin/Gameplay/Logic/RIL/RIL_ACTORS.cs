using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// Actors 渲染指令
    /// </summary>
    public class RIL_ACTORS : IRIL
    {
        public override ushort id => RIL_DEFINE.ACTOR;
        
        /// <summary>
        /// Actor 列表
        /// </summary>
        public List<ulong> actors { get; set; }
        
        protected override void OnReady()
        {
            actors = RILCache.Ensure<List<ulong>>();
        }

        protected override void OnReset()
        {
            actors.Clear();
            RILCache.Set(actors);
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
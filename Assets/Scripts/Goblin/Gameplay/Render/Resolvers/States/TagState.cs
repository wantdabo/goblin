using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 标签状态
    /// </summary>
    public class TagState : State
    {
        public override StateType type => StateType.Tag;
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }
        
        protected override void OnReset()
        {
            if (null != tags)
            {
                tags.Clear();
                ObjectCache.Set(tags);
            }
        }
    }
}
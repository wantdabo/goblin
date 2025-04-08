using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 标签信息, Actor 上的标签信息，用于标记 Actor 各种颗粒度细的信息
    /// </summary>
    public class TagInfo : BehaviorInfo
    {
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }

        protected override void OnReady()
        {
            tags = ObjectCache.Get<Dictionary<ushort, int>>();
        }

        protected override void OnReset()
        {
            tags.Clear();
            ObjectCache.Set(tags);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<TagInfo>();
            foreach (var kv in tags)
            {
                clone.tags.Add(kv.Key, kv.Value);
            }
            
            return clone;
        }
    }
}
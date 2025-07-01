using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.DIFF;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 标签
    /// </summary>
    public class Tag : Behavior<TagInfo>
    {
        /// <summary>
        /// 拥有指定的标签
        /// </summary>
        /// <param name="key">标签 KEY</param>
        /// <returns>YES/NO</returns>
        public bool Has(ushort key)
        {
            return info.tags.ContainsKey(key);
        }
        
        /// <summary>
        /// 获取指定标签的值
        /// </summary>
        /// <param name="key">标签 KEY</param>
        /// <param name="tag">标签值</param>
        /// <returns>YES/NO</returns>
        public bool Get(ushort key, out long tag)
        {
            return info.tags.TryGetValue(key, out tag);
        }
        
        /// <summary>
        /// 移除指定的标签
        /// </summary>
        /// <param name="key">标签 KEY</param>
        public void Rmv(ushort key)
        {
            if (false == info.tags.TryGetValue(key, out long tag)) return;
            
            info.tags.Remove(key);
        }
        
        /// <summary>
        /// 设置指定标签的值
        /// </summary>
        /// <param name="key">标签 KEY</param>
        /// <param name="tag">标签值</param>
        public void Set(ushort key, int tag)
        {
            if (info.tags.ContainsKey(key)) info.tags.Remove(key);
            info.tags.Add(key, tag);
        }
    }
}
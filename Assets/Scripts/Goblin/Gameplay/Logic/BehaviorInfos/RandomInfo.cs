using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 随机信息
    /// </summary>
    public class RandomInfo : BehaviorInfo
    {
        /// <summary>
        /// 乘数
        /// </summary>
        public long a { get; private set; }
        /// <summary>
        /// 增量
        /// </summary>
        public long c { get; private set; }
        /// <summary>
        /// 模数
        /// </summary>
        public long m { get; private set; }
        /// <summary>
        /// 随机种子
        /// </summary>
        public long seed { get; set; }
        /// <summary>
        /// 最新随机数
        /// </summary>
        public long current { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            a = 1664525;
            c = 1013904223;
            m = int.MaxValue;
            seed = 0;
            current = 0;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<RandomInfo>();
            clone.Ready(id);
            clone.seed = seed;
            clone.current = current;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + a.GetHashCode();
            hash = hash * 31 + c.GetHashCode();
            hash = hash * 31 + m.GetHashCode();
            hash = hash * 31 + seed.GetHashCode();
            hash = hash * 31 + current.GetHashCode();

            return hash;
        }
    }
}
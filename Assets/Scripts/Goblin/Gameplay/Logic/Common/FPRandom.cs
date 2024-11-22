using Goblin.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// 确定性，随机器
    /// </summary>
    public class FPRandom : Comp
    {
        private Kowtow.Math.FPRandom random { get; set; }

        /// <summary>
        /// 随机种子
        /// </summary>
        public int seed { get; private set; } = -1;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="s">种子</param>
        public void Initial(int s)
        {
            seed = s;
            random = Kowtow.Math.FPRandom.New(seed);
        }

        /// <summary>
        /// 确定性，浮点数范围随机
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public FP Range(FP min, FP max)
        {
            return random.Next(min.AsFloat(), max.AsFloat());
        }

        /// <summary>
        /// 整数范围随机
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public int Range(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}

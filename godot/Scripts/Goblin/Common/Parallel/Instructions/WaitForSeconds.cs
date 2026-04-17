using Goblin.Common.Parallel.Instructions;

namespace Goblin.Common.Parallel.Instructions
{
    /// <summary>
    /// 等待 N 秒
    /// </summary>
    public class WaitForSeconds : Instruction
    {
        /// <summary>
        /// 秒
        /// </summary>
        public float seconds { get; set; }
        /// <summary>
        /// 流逝的时间
        /// </summary>
        private float elapsed { get; set; }

        /// <summary>
        /// 等待 N 秒
        /// </summary>
        /// <param name="seconds">秒</param>
        public WaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public override void Update(float tick)
        {
            elapsed += tick;
            if (elapsed >= seconds) finished = true;
        }
    }
}

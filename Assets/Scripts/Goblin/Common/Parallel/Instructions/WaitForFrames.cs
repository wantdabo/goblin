namespace Goblin.Common.Parallel.Instructions
{
    /// <summary>
    /// 等待 N 帧
    /// </summary>
    public class WaitForFrames : Instruction
    {
        /// <summary>
        /// 帧
        /// </summary>
        public uint frames { get; set; }
        /// <summary>
        /// 当前记录的帧
        /// </summary>
        private uint curFrame { get; set; }

        /// <summary>
        /// 等待 N 帧
        /// </summary>
        /// <param name="frames">帧</param>
        public WaitForFrames(uint frames)
        {
            this.frames = frames;
        }

        public override void Update(float tick)
        {
            curFrame++;
            if (curFrame >= frames) finished = true;
        }
    }
}

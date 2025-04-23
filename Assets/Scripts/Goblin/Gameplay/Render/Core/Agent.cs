namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 代理
    /// </summary>
    public abstract class Agent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public ulong actor { get; private set; }
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="world">世界</param>
        public void Ready(ulong id, World world)
        {
            this.actor = id;
            this.world = world;
            OnReady();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            this.actor = 0;
            this.world = null;
        }
        
        /// <summary>
        /// 到达
        /// </summary>
        public void Arrive()
        {
            OnArrive();
        }

        /// <summary>
        /// 追逐
        /// </summary>
        /// <param name="tick">tick/s</param>
        /// <param name="timescale">时间缩放</param>
        public void Chase(float tick, float timescale)
        {
            OnChase(tick, timescale);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnReady();
        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void OnReset();
        protected virtual void OnArrive() { }
        protected virtual void OnChase(float tick, float timescale) { }
    }
}
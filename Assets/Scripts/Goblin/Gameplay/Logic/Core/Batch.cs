using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// Batch/批处理, 类似 ECS 中的 System (不推荐写指定单位逻辑, 指定单位逻辑请到 Behavior 中设计)
    /// </summary>
    public abstract class Batch
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>Batch/批处理</returns>
        public Batch Load(Stage stage)
        {
            this.stage = stage;
            OnLoad();
            
            return this;
        }

        public void Unload()
        {
            OnUnload();
            this.stage = null;
        }

        /// <summary>
        /// Tick, 在每一帧中, 会被调用
        /// </summary>
        /// <param name="tick">步长</param>
        public void Tick(FP tick)
        {
            OnTick(tick);
        }
        
        /// <summary>
        /// EndTick, 在全部逻辑帧末, 会被调用
        /// </summary>
        public void EndTick()
        {
            OnEndTick();
        }

        /// <summary>
        /// 加载时调用, 子类重写
        /// </summary>
        protected virtual void OnLoad()
        {
        }

        /// <summary>
        /// 卸载时调用, 子类重写
        /// </summary>
        protected virtual void OnUnload()
        {
        }

        /// <summary>
        /// Tick, 子类重写
        /// </summary>
        /// <param name="tick">步长</param>
        protected virtual void OnTick(FP tick)
        {
        }
        
        /// <summary>
        /// EndTick, 子类重写
        /// </summary>
        protected virtual void OnEndTick()
        {
        }
    }
}
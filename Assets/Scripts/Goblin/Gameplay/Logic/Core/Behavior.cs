using Goblin.Gameplay.Logic.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// Behavior/行为, 类似 ECS 中的 System
    /// </summary>
    public abstract class Behavior
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stage">场景</param>
        /// <param name="actor">ActorID</param>
        public void Initialize(Stage stage, ulong actor)
        {
            this.stage = stage;
            this.actor = actor;
        }

        /// <summary>
        /// 组装
        /// </summary>
        public void Assemble()
        {
            OnAssemble();
        }
        
        /// <summary>
        /// 拆解
        /// </summary>
        public void Disassemble()
        {
            OnDisassemble();
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
        /// 组装, 子类重写
        /// </summary>
        protected virtual void OnAssemble()
        {
        }

        /// <summary>
        /// 拆解, 子类重写
        /// </summary>
        protected virtual void OnDisassemble()
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

    /// <summary>
    /// Behavior/行为, 类似 ECS 中的 System
    /// </summary>
    /// <typeparam name="T">BehaviorInfo 类型</typeparam>
    public abstract class Behavior<T> : Behavior where T : BehaviorInfo, new()
    {
        /// <summary>
        /// BehaviorInfo 快捷访问
        /// </summary>
        public T info => stage.GetBehaviorInfo<T>(actor);

        protected override void OnAssemble()
        {
            base.OnAssemble();
            // Behavior<T> 实现类, 可以指定 BehaviorInfo 用来快速访问对应的 BehaviorInfo
            if (false == stage.SeekBehaviorInfo(actor, out T info)) stage.AddBehaviorInfo<T>(actor);
        }
    }
}
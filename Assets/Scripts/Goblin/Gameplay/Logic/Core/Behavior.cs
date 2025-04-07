using Goblin.Gameplay.Logic.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 行为信息, 类似 ECS 中的 Component
    /// </summary>
    public abstract class BehaviorInfo
    {
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong id { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void Ready(ulong actor)
        {
            this.id = actor;
            OnReady();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            this.id = 0;
        }

        /// <summary>
        /// 初始化, 当 BehaviorInfo 从对象池中取出, 在这个回调中初始化数据
        /// </summary>
        protected abstract void OnReady();
        
        /// <summary>
        /// 重置, 当 BehaviorInfo 回收, 重新回到对象池, 在这个回调中清理数据
        /// </summary>
        protected abstract void OnReset();
    }
    
    /// <summary>
    /// Behavior/行为, 类似 ECS 中的 System, 可是非批处理, 它是对应 Actor 中具体的逻辑
    /// </summary>
    public abstract class Behavior
    {
        /// <summary>
        /// 实体
        /// </summary>
        public Actor actor { get; private set; }
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }

        /// <summary>
        /// 组装, 当一个 Behavior 被访问到, 会自动组装
        /// </summary>
        /// <param name="stage">场景</param>
        /// <param name="actor">Actor/实体</param>
        public void Assemble(Stage stage, Actor actor)
        {
            this.actor = actor;
            this.stage = stage;
            OnAssemble();
        }
        
        /// <summary>
        /// 拆解, 在帧末的时机, Actor 会被拆解回到对象池
        /// </summary>
        public void Disassemble()
        {
            OnDisassemble();
            this.actor = null;
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
    /// Behavior/行为, 类似 ECS 中的 System, 可是非批处理, 它是对应 Actor 单一的逻辑
    /// </summary>
    /// <typeparam name="T">BehaviorInfo 类型</typeparam>
    public abstract class Behavior<T> : Behavior where T : BehaviorInfo, new()
    {
        /// <summary>
        /// BehaviorInfo 快捷访问
        /// </summary>
        public T info { get; private set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            // Behavior<T> 实现类, 可以指定 BehaviorInfo 用来快速访问对应的 BehaviorInfo
            // 自动获取 BehaviorInfo
            info = actor.stage.GetBehaviorInfo<T>(actor.id);
            if (null == info) info = stage.AddBehaviorInfo<T>(actor.id);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            info = default;
        }
    }
}
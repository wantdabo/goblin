using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Luban;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// Actor/实体, 类似 ECS 中的 Entity.
    /// </summary>
    public sealed class Actor
    {
        /// <summary>
        /// ID
        /// </summary>
        public ulong id { get; private set; }
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; private set; }
        /// <summary>
        /// 事件订阅器
        /// </summary>
        public Eventor eventor => GetBehavior<Eventor>();
        
        /// <summary>
        /// 组装
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="stage">场景</param>
        public void Assemble(ulong id, Stage stage)
        {
            this.id = id;
            this.stage = stage;
        }
        
        /// <summary>
        /// 拆解
        /// </summary>
        public void Disassemble()
        {
            this.id = 0;
            this.stage = null;
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="type">Behavior 类型</param>
        /// <returns>Behavior</returns>
        public Behavior GetBehavior(Type type)
        {
            return stage.GetBehavior(id, type);
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        public T GetBehavior<T>() where T : Behavior
        {
            return stage.GetBehavior<T>(id);
        }

        /// <summary>
        /// 寻找 Behavior
        /// </summary>
        /// <param name="behavior">Behavior</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehavior<T>(out T behavior) where T : Behavior, new()
        {
            return stage.SeekBehavior<T>(id, out behavior);
        }
        
        public bool SeekBehavior(Type type, out Behavior behavior)
        {
            return stage.SeekBehavior(id, type, out behavior);
        }
        
        /// <summary>
        /// 添加 Behavior
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">一个 Actor 不能添加多个同种 Behavior</exception>
        public T AddBehavior<T>() where T : Behavior, new()
        {
            return stage.AddBehavior<T>(id);
        }
        
        /// <summary>
        /// 获取 BehaviorInfo
        /// </summary>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>BehaviorInfo</returns>
        public T GetBehaviorInfo<T>() where T : BehaviorInfo
        {
            return stage.GetBehaviorInfo<T>(id);
        }
        
        /// <summary>
        /// 寻找 BehaviorInfo
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviorInfo<T>(out T info) where T : BehaviorInfo
        {
            return stage.SeekBehaviorInfo<T>(id, out info);
        }

        /// <summary>
        /// 添加 BehaviorInfo
        /// </summary>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>BehaviorInfo</returns>
        public T AddBehaviorInfo<T>() where T : BehaviorInfo, new()
        {
            return stage.AddBehaviorInfo<T>(id);
        }
    }
}
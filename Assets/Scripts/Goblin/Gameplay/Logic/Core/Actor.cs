using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Luban;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 实体
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
        
        public void Assemble(ulong id, Stage stage)
        {
            this.id = id;
            this.stage = stage;
        }
        
        public void Disassemble()
        {
            this.id = 0;
            this.stage = null;
        }
        
        public bool SeekBehavior<T>(out T behavior) where T : Behavior, new()
        {
            return stage.SeekBehavior<T>(id, out behavior);
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

        public bool SeekBehavior(Type type, out Behavior behavior)
        {
            return stage.SeekBehavior(id, type, out behavior);
        }
        
        public bool SeekBehaviorInfo<T>(out T info) where T : IBehaviorInfo
        {
            return stage.SeekBehaviorInfo<T>(id, out info);
        }

        public T AddBehaviorInfo<T>() where T : IBehaviorInfo, new()
        {
            return stage.AddBehaviorInfo<T>(id);
        }
    }
}
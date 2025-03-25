using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;

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
        /// <summary>
        /// 驱动器
        /// </summary>
        public Ticker ticker { get; private set; }
        
        public void Assemble(ulong id, Stage stage)
        {
            this.id = id;
            this.stage = stage;
            this.ticker = GetBehavior<Ticker>();
            if (null == this.ticker)
            {
                AddBehavior<Ticker>();
                this.ticker = GetBehavior<Ticker>();
            }
        }
        
        public void Disassemble()
        {
            this.id = 0;
            this.stage = null;
            this.ticker = null;
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
        public T GetBehavior<T>() where T : Behavior, new()
        {
            return stage.GetBehavior<T>(id);
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
    }
}
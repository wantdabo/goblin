using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Lives;
using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// Actor/实体
    /// </summary>
    public class Actor : Comp
    {
        /// <summary>
        /// ActorID/实体 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; set; }
        /// <summary>
        /// 确定性，Ticker/时间驱动器
        /// </summary>
        public FPTicker ticker { get; set; }
        /// <summary>
        /// Behavior 集合
        /// </summary>
        private Dictionary<Type, Behavior> behaviorDict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();

            ticker = AddComp<FPTicker>();
            ticker.Create();
            
            stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            
            AddBehavior<Live>().Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        public T GetBehavior<T>() where T : Behavior
        {
            if (null == behaviorDict) return default;

            if (behaviorDict.TryGetValue(typeof(T), out var behavior)) return behavior as T;

            return default;
        }

        /// <summary>
        /// 添加 Behavior
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">一个 Actor 不能添加多个同种 Behavior</exception>
        public T AddBehavior<T>() where T : Behavior, new()
        {
            if (null == behaviorDict) behaviorDict = new();

            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception($"can't add same behavior -> {typeof(T)}");

            var behavior = AddComp<T>();
            behavior.actor = this;
            behaviorDict.Add(typeof(T), behavior);

            return behavior;
        }

        /// <summary>
        /// 移除 Behavior
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        public void RmvBehavior<T>() where T : Behavior
        {
            RmvBehavior(GetBehavior<T>());
        }

        /// <summary>
        /// 移除 Behavior
        /// </summary>
        /// <param name="behavior">Behavior 实体</param>
        public void RmvBehavior(Behavior behavior)
        {
            if (null == behaviorDict) return;
            behaviorDict.Remove(behavior.GetType());
        }
        
        private void OnFPTick(FPTickEvent e)
        {
            ticker.Tick();
        }
    }
}
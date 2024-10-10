using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Resolvers;
using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Render.Core
{
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
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Behavior 集合
        /// </summary>
        private Dictionary<Type, Behavior> behaviorDict { get; set; }
        /// <summary>
        /// Resolver 集合
        /// </summary>
        private Dictionary<ushort, Resolver> resolverDict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();

            eventor.Listen<RILResolveEvent>(OnRILResolve);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventor.UnListen<RILResolveEvent>(OnRILResolve);
        }

        public T EnsureBehavior<T>() where T : Behavior, new()
        {
            var behavior = GetBehavior<T>();
            if (null == behavior)
            {
                behavior = AddBehavior<T>();
                behavior.Create();
            }

            return behavior;
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
        private T AddBehavior<T>() where T : Behavior, new()
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
        private void RmvBehavior<T>() where T : Behavior
        {
            RmvBehavior(GetBehavior<T>());
        }

        /// <summary>
        /// 移除 Behavior
        /// </summary>
        /// <param name="behavior">Behavior 实体</param>
        private void RmvBehavior(Behavior behavior)
        {
            if (null == behaviorDict) return;
            behaviorDict.Remove(behavior.GetType());
            behavior.Destroy();
        }

        private Resolver GetResolver(ushort id)
        {
            if (null == resolverDict) return default;

            return resolverDict.GetValueOrDefault(id);
        }

        private Resolver AddResolver<T>(ushort id) where T : Resolver, new()
        {
            if (null == resolverDict) resolverDict = new();

            if (resolverDict.ContainsKey(id)) throw new Exception($"can't add same resolver -> {typeof(T)}");

            var resolver = AddComp<T>();
            resolver.actor = this;
            resolver.Create();
            resolverDict.Add(id, resolver);

            return resolver;
        }

        private void RmvResolver(ushort id)
        {
            var resolver = GetResolver(id);
            if (null == resolver) return;
            resolverDict.Remove(id);
            resolver.Destroy();
        }

        private void OnRILResolve(RILResolveEvent e)
        {
            var resolver = GetResolver(e.ril.id);
            if (null == resolver)
            {
                switch (e.ril.id)
                {
                    case RILDef.SPATIAL_POSITION:
                        resolver = AddResolver<SpatialPosition>(e.ril.id);
                        break;
                    case RILDef.SPATIAL_ROTATION:
                        resolver = AddResolver<SpatialRotation>(e.ril.id);
                        break;
                    case RILDef.SPATIAL_SCALE:
                        resolver = AddResolver<SpatialScale>(e.ril.id);
                        break;
                    case RILDef.STATEMACHINE_ZERO:
                        resolver = AddResolver<StateMachineZero>(e.ril.id);
                        break;
                    case RILDef.STATEMACHINE_ONE:
                        resolver = AddResolver<StateMachineOne>(e.ril.id);
                        break;
                    case RILDef.ATTR_SURFACE:
                        resolver = AddResolver<AttrSurface>(e.ril.id);
                        break;
                }

                if (null == resolver) return;
                resolver.Create();
                resolver.Awake(e.frame, e.ril);
            }

            resolver.Resolve(e.frame, e.ril);
        }
    }
}

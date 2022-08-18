using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Theaters;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors
{
    public class Actor : PComp
    {
        public Theater Theater;
        public ActorBehavior ActorBehavior;

        protected override void OnCreate()
        {
            ActorBehavior = AddBehavior<ActorBehavior>();
            base.OnCreate();
        }

        private Dictionary<Type, PComp> behaviorDict = new Dictionary<Type, PComp>();

        /// <summary>
        /// 获取行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>行为逻辑组件</returns>
        public T GetBehavior<T>() where T : PComp, new()
        {
            if (behaviorDict.TryGetValue(typeof(T), out var behavior)) return behavior as T;

            return null;
        }

        /// <summary>
        /// 移除行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <typeparam name="TI">行为逻辑组件数据类型</typeparam>
        public void RmvBehavior<T, TI>() where T : Behavior<TI>, new() where TI : BehaviorInfo, new()
        {
            if (false == behaviorDict.TryGetValue(typeof(T), out var behavior)) return;
            behaviorDict.Remove(typeof(T));
            RmvComp(behavior);
        }

        /// <summary>
        /// 添加行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// 
        public T AddBehavior<T>() where T : PComp, new()
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception("can't add same behavior to one actor");

            var comp = AddComp<T>();
            behaviorDict.Add(typeof(T), comp);

            return comp;
        }

        private List<Actor> actorList = new List<Actor>();
        private Dictionary<int, Actor> actorDict = new Dictionary<int, Actor>();

        /// <summary>
        /// 获取 Actor
        /// </summary>
        /// <param name="actorId">actorId 唯一身份表示</param>
        /// <returns>Actor</returns>
        public Actor GetActor(int actorId)
        {
            actorDict.TryGetValue(actorId, out var actor);

            return actor;
        }

        /// <summary>
        /// 移除 Actor
        /// </summary>
        /// <param name="actor">Actor</param>
        public void RmvActor(Actor actor)
        {
            actorList.Remove(actor);
            RmvComp(actor);
        }

        /// <summary>
        /// 添加实体组件
        /// </summary>
        /// <typeparam name="T">实体组件类型</typeparam>
        /// <returns>Actor</returns>
        public T AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>((item) =>
            {
                item.Theater = Theater;
            });
            actorList.Add(actor);
            actorDict.Add(actor.ActorBehavior.Info.actorId, actor);

            return actor;
        }

        private new T AddComp<T>(Action<T> createAheadAction = null) where T : PComp, new()
        {
            return base.AddComp<T>((item) =>
            {
                item.Actor = this;
                createAheadAction?.Invoke(item);
            });
        }
    }
}

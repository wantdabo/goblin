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
        public Theater theater;
        public ActorBehavior actorBehaivor;

        protected override void OnCreate()
        {
            actorBehaivor = AddBehavior<ActorBehavior>();
            base.OnCreate();
        }

        private Dictionary<Type, Behavior> behaviorDict = new Dictionary<Type, Behavior>();

        /// <summary>
        /// 获取行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>行为逻辑组件</returns>
        public T GetBehavior<T>() where T : Behavior, new()
        {
            if (behaviorDict.TryGetValue(typeof(T), out var behavior)) return behavior as T;

            return null;
        }

        public void UnBindingBehavior<T>() where T : Behavior
        {
            behaviorDict.Remove(typeof(T));
        }

        public void BindingBehavior<T>(Behavior behavior) where T : Behavior, new()
        {
            behaviorDict.Add(typeof(T), behavior);
        }

        /// <summary>
        /// 移除行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        public void RmvBehavior<T>() where T : Behavior, new()
        {
            if (false == behaviorDict.TryGetValue(typeof(T), out var behavior)) return;
            UnBindingBehavior<T>();
            RmvComp(behavior);
        }

        /// <summary>
        /// 添加行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>Behavior</returns>
        public T AddBehavior<T>() where T : Behavior, new()
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception("can't add same behavior to one actor");

            var behavior = AddComp<T>();
            BindingBehavior<T>(behavior);

            return behavior;
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
                item.theater = theater;
            });
            actorList.Add(actor);
            actorDict.Add(actor.actorBehaivor.info.actorId, actor);

            return actor;
        }

        public override T AddComp<T>(Action<T> createAheadAction = null)
        {
            return base.AddComp<T>((item) =>
            {
                item.actor = this;
                createAheadAction?.Invoke(item);
            });
        }
    }
}

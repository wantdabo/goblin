using GoblinFramework.Core;
using GoblinFramework.Gameplay.Actors;
using GoblinFramework.Gameplay.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// Logic-Comp，逻辑层组件
    /// </summary>
    public class LComp : Comp<PGEngine>
    {
        public Actor Actor;

        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IPLoop) Engine.TickEngine.AddPLoop(this as IPLoop);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IPLoop) Engine.TickEngine.RmvPLoop(this as IPLoop);
        }

        /// <summary>
        /// 添加实体组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T AddActor<T>() where T : Actor, new()
        {
            var actor = base.AddComp<T>();
            actor.Actor = Actor;
            if (actor is Actor) return actor;

            throw new Exception("AddActor only can to add Actor, if you want to add Actor, plz use Actor.AddActor");
        }

        private Dictionary<Type, LComp> behaviorDict = new Dictionary<Type, LComp>();

        /// <summary>
        /// 获取行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>行为逻辑组件</returns>
        public T GetBehavior<T>() where T : LComp, new() 
        {
            if (behaviorDict.TryGetValue(typeof(T), out var behavior)) return behavior as T;

            return null;
        }

        /// <summary>
        /// 添加行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>行为逻辑组件</returns>
        public T AddBehavior<T>() where T : LComp, new() 
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception("can't add same behavior to one actor");

            var comp = base.AddComp<T>();
            behaviorDict.Add(typeof(T), comp);

            if (comp is Actor) throw new Exception("AddBehavior only can to add BehaviorComp, if you want to add Actor, plz use Actor.AddActor");
            comp.Actor = Actor;

            return comp;
        }

        /// <summary>
        /// 禁用 AddComp，如果需要添加组件，请使用 AddBehavior/AddActor
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>NULL</returns>
        /// <exception cref="Exception">请使用 AddBehavior/AddActor 去完成工作</exception>
        public override T AddComp<T>()
        {
            throw new Exception("plz use AddBehavior to finish your work.");
        }
    }
}

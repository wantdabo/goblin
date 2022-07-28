using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors
{
    public class Actor : LComp
    {
        public ActorInfo ActorInfo = new ActorInfo();

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
        /// 移除行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <typeparam name="TI">行为逻辑组件数据类型</typeparam>
        public void RmvBehavior<T, TI>() where T : Behavior<TI>, new() where TI : LInfo, new()
        {
            if (false == behaviorDict.TryGetValue(typeof(T), out var behavior)) return;
            behaviorDict.Remove(typeof(T));
            RmvComp(behavior);
        }

        /// <summary>
        /// 添加行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        public T AddBehavior<T>() where T : LComp, new()
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception("can't add same behavior to one actor");

            var comp = AddComp<T>();
            behaviorDict.Add(typeof(T), comp);
            comp.Actor = Actor;

            return comp;
        }

        public void RmvActor(Actor actor)
        {
            actorList.Remove(actor);
            RmvComp(actor);
        }

        private List<Actor> actorList = new List<Actor>();

        /// <summary>
        /// 添加实体组件
        /// </summary>
        /// <typeparam name="T">实体组件类型</typeparam>
        /// <returns></returns>
        public T AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>();
            actorList.Add(actor);
            actor.Actor = Actor;

            return actor;
        }
    }

    #region ActorInfo
    public class ActorInfo : LInfo
    {
        public int actorId;
        public int angle;
        public Fixed64Vector3 pos;

        public override object Clone()
        {
            var actorInfo = new ActorInfo();
            actorInfo.actorId = actorId;
            actorInfo.pos = pos;

            return actorInfo;
        }
    }
    #endregion
}

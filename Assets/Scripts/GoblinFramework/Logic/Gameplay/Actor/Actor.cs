using GoblinFramework.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Logic.Gameplay
{
    public class Actor : LComp
    {
        public ActorBehavior actorBehaivor;
        private Dictionary<Type, Behavior> behaviorDict = new Dictionary<Type, Behavior>();

        protected override void OnCreate()
        {
            base.OnCreate();
            actorBehaivor = AddBehavior<ActorBehavior>();
        }

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

        /// <summary>
        /// 添加行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        /// <returns>Behavior</returns>
        public T AddBehavior<T>() where T : Behavior, new()
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception("can't add same behavior to one actor");

            var behavior = AddComp<T>();
            behavior.actor = this;
            behavior.Create();

            return behavior;
        }

        /// <summary>
        /// 移除行为逻辑组件
        /// </summary>
        /// <typeparam name="T">行为逻辑组件类型</typeparam>
        public void RmvBehavior<T>() where T : Behavior, new()
        {
            if (false == behaviorDict.TryGetValue(typeof(T), out var behavior)) return;
            RmvComp(behavior);
            behavior.Destroy();
        }
    }
}

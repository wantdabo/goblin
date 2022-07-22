#if GOBLIN_CLIENT
using GoblinFramework.Client;
using GoblinFramework.Client.Common;
#endif
using GoblinFramework.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    /// <summary>
    /// 组件，核心思想类
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    public abstract class Comp<E> : Goblin where E : GameEngineComp<E>, new()
    {
        /// <summary>
        /// 组件列表
        /// </summary>
        private List<Comp<E>> compList = new List<Comp<E>>();
        /// <summary>
        /// 组件字典，根据组件类型分类
        /// </summary>
        private Dictionary<Type, List<Comp<E>>> compDict = new Dictionary<Type, List<Comp<E>>>();

        /// <summary>
        /// 父组件
        /// </summary>
        protected Comp<E> parent;

        /// <summary>
        /// 引擎组件
        /// </summary>
        public E Engine;

        protected override void OnCreate()
        {
#if GOBLIN_CLIENT
            if (Engine is CGEngineComp) 
            {
                var engine = Engine as CGEngineComp;
                if (this is IUpdate) engine.TickEngine.AddUpdate(this as IUpdate);
                if (this is ILateUpdate) engine.TickEngine.AddLateUpdate(this as ILateUpdate);
                if (this is IFixedUpdate) engine.TickEngine.AddFixedUpdate(this as IFixedUpdate);
            }
#endif
        }

        protected override void OnDestroy()
        {
#if GOBLIN_CLIENT
            if (Engine is CGEngineComp)
            {
                var engine = Engine as CGEngineComp;
                if (this is IUpdate) engine.TickEngine.RmvUpdate(this as IUpdate);
                if (this is ILateUpdate) engine.TickEngine.RmvLateUpdate(this as ILateUpdate);
                if (this is IFixedUpdate) engine.TickEngine.RmvFixedUpdate(this as IFixedUpdate);
            }
#endif
            for (int i = compList.Count - 1; i >= 0; i--) RmvComp(compList[i]);
            compList.Clear();
            compList = null;

            compDict.Clear();
            compDict = null;
        }

        /// <summary>
        /// 获取已挂载的指定类型的组件列表
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="force">强制查询，如果字段快速查询未找到，并且 force 为 ture，将以高代价的查询方式获取</typeparam>
        /// <returns>组件列表</returns>
        public virtual List<T> GetComp<T>(bool force = false) where T : Comp<E>
        {
            if (compDict.TryGetValue(typeof(T), out List<Comp<E>> comps)) return comps as List<T>;
            if (false == force) return null;

            List<T> list = null;
            foreach (var comp in compList)
            {
                if (comp is T) 
                {
                    if(null == list) list = new List<T>();
                    list.Add(comp as T);
                }
            }

            return list;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>返回一个就绪的组件</returns>
        public virtual T AddComp<T>() where T : Comp<E>, new()
        {
            T comp = new T();

            if (false == compDict.TryGetValue(typeof(T), out List<Comp<E>> comps))
            {
                comps = new List<Comp<E>>();
                compDict.Add(typeof(T), comps);
            }
            comps.Add(comp);
            compList.Add(comp);

            comp.Engine = Engine;
            comp.parent = this;

            comp.Create();

            return comp;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="comp">组件</param>
        public virtual void RmvComp(Comp<E> comp)
        {
            comp.Destroy();

            if (compDict.TryGetValue(comp.GetType(), out List<Comp<E>> comps)) comps.Remove(comp);
            compList.Remove(comp);
        }

        /// <summary>
        /// 移除已挂载指定类型的所有组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        public virtual void RmvComp<T>()
        {
            if (compDict.TryGetValue(typeof(T), out List<Comp<E>> comps)) for (int i = comps.Count - 1; i >= 0; i--) RmvComp(comps[i]);
        }
    }
}

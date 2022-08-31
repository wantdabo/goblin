using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    public abstract class Comp : Goblin
    {
        protected Comp parent;
        /// <summary>
        /// 组件列表
        /// </summary>
        private List<Comp> compList = new List<Comp>();

        /// <summary>
        /// 组件字典，根据组件类型分类
        /// </summary>
        private Dictionary<Type, List<Comp>> compsDict = new Dictionary<Type, List<Comp>>();

        protected override void OnCreate()
        {
        }

        protected override void OnDestroy()
        {
            for (int i = compList.Count - 1; i >= 0; i--) RmvComp(compList[i]);
            compList.Clear();
            compList = null;

            compsDict.Clear();
            compsDict = null;
        }

        /// <summary>
        /// 获取已挂载的指定类型的组件列表
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="force">强制查询，如果字段快速查询未找到，并且 force 为 ture，将以高代价的查询方式获取</typeparam>
        /// <returns>组件列表</returns>
        public virtual List<T> GetComps<T>(bool force = false) where T : Comp
        {
            List<T> list = null;
            if (compsDict.TryGetValue(typeof(T), out List<Comp> comps))
            {
                list = new List<T>();

                foreach (var item in comps) list.Add(item as T);

                return list;
            }

            if (false == force) return null;

            // 高代价查表
            foreach (var comp in compList)
            {
                if (comp is T)
                {
                    if (null == list) list = new List<T>();
                    list.Add(comp as T);
                }
            }

            return list;
        }

        /// <summary>
        /// 获得一个组件，如果存在多个，将会取到最新那个
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件</returns>
        public virtual T GetComp<T>() where T : Comp
        {
            var comps = GetComps<T>();
            if (null == comps) return null;

            return comps.Last();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="createAheadAction"></param>
        /// <returns>组件</returns>
        public virtual T AddComp<T>(Action<T> createAheadAction = null) where T : Comp, new()
        {
            T comp = new T();

            if (false == compsDict.TryGetValue(typeof(T), out List<Comp> comps))
            {
                comps = new List<Comp>();
                compsDict.Add(typeof(T), comps);
            }
            comps.Add(comp);
            compList.Add(comp);

            comp.parent = this;

            createAheadAction?.Invoke(comp);
            comp.Create();

            return comp;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="comp">组件</param>
        public virtual void RmvComp(Comp comp)
        {
            if (compsDict.TryGetValue(comp.GetType(), out List<Comp> comps)) comps.Remove(comp);
            compList.Remove(comp);

            comp.Destroy();
        }

        /// <summary>
        /// 移除已挂载指定类型的所有组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        public virtual void RmvComp<T>()
        {
            if (compsDict.TryGetValue(typeof(T), out List<Comp> comps)) for (int i = comps.Count - 1; i >= 0; i--) RmvComp(comps[i]);
        }
    }

    /// <summary>
    /// 组件，核心思想类
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    public abstract class Comp<E> : Comp where E : GameEngine<E>, new()
    {
        /// <summary>
        /// 引擎组件
        /// </summary>
        public E Engine;

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="createAheadAction"></param>
        /// <returns>返回一个就绪的组件</returns>
        public override T AddComp<T>(Action<T> createAheadAction = null)
        {
            return base.AddComp<T>((comp) =>
            {
                var engineComp = (comp as Comp<E>);
                if (null != engineComp) engineComp.Engine = Engine;

                createAheadAction?.Invoke(comp);
            });
        }
    }
}

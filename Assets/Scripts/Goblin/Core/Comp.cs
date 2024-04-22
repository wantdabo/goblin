using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Core
{
    /// <summary>
    /// 组件
    /// </summary>
    public abstract class Comp : Goblin
    {
        /// <summary>
        /// 引擎
        /// </summary>
        public Engine engine;

        /// <summary>
        /// 组件列表
        /// </summary>
        private List<Comp> compList = null;

        /// <summary>
        /// 组件字典，根据组件类型分类
        /// </summary>
        private Dictionary<Type, List<Comp>> compDict = null;


        protected override void OnCreate()
        {
        }

        protected override void OnDestroy()
        {
            if (null == compList) return;
            for (int i = compList.Count - 1; i >= 0; i--)
            {
                var comp = compList[i];
                RmvComp(comp);
                comp.Destroy();
            }
            compList.Clear();
            compList = null;

            compDict.Clear();
            compDict = null;
        }

        /// <summary>
        /// 获取已挂载的指定类型的组件列表
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="force">强制查询，如果字段快速查询未找到，并且 force 为 true，将以高代价的查询方式获取</typeparam>
        /// <returns>组件列表</returns>
        public virtual List<T> GetComps<T>(bool force = false) where T : Comp
        {
            List<T> list = null;
            if (compDict.TryGetValue(typeof(T), out var comps))
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
                    if (null == list) list = new();
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
        public virtual T GetComp<T>(bool force = false) where T : Comp
        {
            var comps = GetComps<T>(force);
            if (null == comps) return null;

            return comps.Last();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件</returns>
        public virtual T AddComp<T>() where T : Comp, new()
        {
            if (null == compList) compList = new();
            if (null == compDict) compDict = new();

            T comp = new();
            comp.engine = engine;
            if (false == compDict.TryGetValue(typeof(T), out var comps))
            {
                comps = new();
                compDict.Add(typeof(T), comps);
            }
            comps.Add(comp);
            compList.Add(comp);

            return comp;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="comp">组件</param>
        public virtual void RmvComp(Comp comp)
        {
            if (compDict.TryGetValue(comp.GetType(), out var comps)) comps.Remove(comp);
            compList.Remove(comp);
        }
    }
}

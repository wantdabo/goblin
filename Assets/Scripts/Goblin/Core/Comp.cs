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
    public abstract class Comp
    {
        /// <summary>
        /// 销毁了
        /// </summary>
        public bool destroyed { get; private set; } = false;

        /// <summary>
        /// 引擎
        /// </summary>
        public Engine engine { get; set; }

        /// <summary>
        /// 父组件
        /// </summary>
        public Comp parent { get; set; }

        /// <summary>
        /// 组件列表
        /// </summary>
        private List<Comp> comps { get; set; }

        /// <summary>
        /// 组件字典，根据组件类型分类
        /// </summary>
        private Dictionary<Type, List<Comp>> compdict { get; set; }

        /// <summary>
        /// 创建一个 Goblin 对象
        /// </summary>
        public void Create()
        {
            OnCreate();
        }

        /// <summary>
        /// 销毁一个 Goblin 对象
        /// </summary>
        public void Destroy()
        {
            if (destroyed) return;
            destroyed = true;
            
            OnDestroy();
            parent.RmvComp(this);
            
            if (null != comps)
            {
                for (int i = comps.Count - 1; i >= 0; i--)
                {
                    comps[i].Destroy();
                }
                comps.Clear();
                compdict.Clear();
            }
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// 获得一个组件，如果存在多个，将会取到最新那个
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件</returns>
        public T GetComp<T>(bool force = false) where T : Comp
        {
            if (false == compdict.TryGetValue(typeof(T), out var comps)) return null;

            return comps.Last() as T;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件</returns>
        public T AddComp<T>() where T : Comp, new()
        {
            if (null == comps) comps = new();
            if (null == compdict) compdict = new();

            T comp = new();
            comp.engine = engine;
            comp.parent = this;
            if (false == compdict.TryGetValue(typeof(T), out var list))
            {
                list = new();
                compdict.Add(typeof(T), list);
            }
            list.Add(comp);
            comps.Add(comp);

            return comp;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="comp">组件</param>
        public void RmvComp(Comp comp)
        {
            if (false == comps.Contains(comp)) return;
            
            if (compdict.TryGetValue(comp.GetType(), out var list)) list.Remove(comp);
            comps.Remove(comp);
        }
    }
}

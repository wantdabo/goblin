using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Core;

/// <summary>
/// 组件
/// </summary>
public abstract class Comp
{
    /// <summary>
    /// 引擎
    /// </summary>
    public Engine engine { get; set; }

    /// <summary>
    /// 父组件
    /// </summary>
    private Comp parent { get; set; }

    /// <summary>
    /// 组件列表
    /// </summary>
    private List<Comp> comps { get; set; } = null;

    /// <summary>
    /// 组件字典，根据组件类型分类
    /// </summary>
    private Dictionary<Type, List<Comp>> compdict { get; set; } = null;

    /// <summary>
    /// 创建一个 Queen 对象
    /// </summary>
    public void Create()
    {
        engine.EnsureThread();
        OnCreate();
    }

    protected virtual void OnCreate()
    {
    }

    /// <summary>
    /// 销毁一个 Queen 对象
    /// </summary>
    public void Destroy()
    {
        engine.EnsureThread();
        
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

    protected virtual void OnDestroy()
    {

    }

    /// <summary>
    /// 获得一个组件，如果存在多个，将会取到最新那个
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件</returns>
    public virtual T GetComp<T>(bool force = false) where T : Comp
    {
        if (false == compdict.TryGetValue(typeof(T), out var comps)) return null;

        return comps.Last() as T;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件</returns>
    public virtual T AddComp<T>() where T : Comp, new()
    {
        engine.EnsureThread();
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
    public virtual void RmvComp(Comp comp)
    {
        engine.EnsureThread();
        if (compdict.TryGetValue(comp.GetType(), out var list)) list.Remove(comp);
        comps.Remove(comp);
    }
}

/// <summary>
/// 组件
/// </summary>
/// <typeparam name="T">引擎类型</typeparam>
public abstract class Comp<T> : Comp where T : Engine, new()
{
    /// <summary>
    /// 引擎
    /// </summary>
    public new T engine { get { return base.engine as T; } }
}
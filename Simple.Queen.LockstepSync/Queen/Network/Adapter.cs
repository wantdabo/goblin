using Queen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Network;

/// <summary>
/// 网络消息绑定特征
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class NetBinding : Attribute
{
}

/// <summary>
/// 消息方法结构体
/// </summary>
public struct ActionInfo
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public Type msgType;
    /// <summary>
    /// 消息回调
    /// </summary>
    public Delegate action;
}

/// <summary>
/// 消息适配器
/// </summary>
public abstract class Adapter : Comp
{
    /// <summary>
    /// 桥接
    /// </summary>
    protected Comp bridge;

    /// <summary>
    /// 消息方法回调
    /// </summary>
    public List<ActionInfo> actionInfos = new();

    protected override void OnCreate()
    {
        base.OnCreate();
        OnBind();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnUnbind();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="bridge">桥接</param>
    public void Initialize(Comp bridge)
    {
        this.bridge = bridge;
    }

    /// <summary>
    /// 绑定消息回调
    /// </summary>
    protected virtual void OnBind() { }

    /// <summary>
    /// 松绑消息回调
    /// </summary>
    protected virtual void OnUnbind() { }
}

/// <summary>
/// 消息适配器
/// </summary>
/// <typeparam name="T">桥接类型</typeparam>
public abstract class Adapter<T> : Adapter where T : Comp
{
    /// <summary>
    /// 桥接
    /// </summary>
    protected new T bridge { get { return base.bridge as T; } }
}

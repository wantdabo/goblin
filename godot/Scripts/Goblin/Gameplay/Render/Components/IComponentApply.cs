using System;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// Component ApplyTo 注册接口
/// SG 生成的 partial 类实现此接口，Mirror.Register 通过泛型约束直接调用
/// </summary>
public interface IComponentApply<T>
    where T : Component
{
    /// <summary>
    /// ApplyTo 委托，将 values 数组应用到 Component 属性
    /// </summary>
    static abstract Action<object, ulong, object[]> ApplyTo { get; }
}

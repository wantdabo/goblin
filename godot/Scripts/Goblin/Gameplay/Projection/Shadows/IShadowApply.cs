using System;

namespace Goblin.Gameplay.Projection.Shadows;

/// <summary>
/// Shadow ApplyTo 注册接口
/// SG 生成的 partial 类实现此接口，Canvas.Register 通过泛型约束直接调用
/// </summary>
public interface IShadowApply<T>
    where T : Shadow
{
    /// <summary>
    /// ApplyTo 委托，将 values 数组应用到 Shadow 属性
    /// T 约束为 Shadow，调用方通过 Register<> 包装为 Action&lt;object,...&gt;
    /// </summary>
    static abstract Action<T, ulong, object[]> ApplyTo { get; }
}

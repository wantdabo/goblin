namespace Goblin.Common;

/// <summary>
/// 池化对象接口
/// 提供 Reset 和 Clone 多态契约
/// Source Generator 扫描 partial class + IGBL 自动生成 override Reset / Clone
/// </summary>
public interface IGBL
{
    /// <summary>
    /// 重置对象状态，回收前调用
    /// </summary>
    void Reset();

    /// <summary>
    /// 深拷贝，返回新实例
    /// </summary>
    IGBL Clone();
}

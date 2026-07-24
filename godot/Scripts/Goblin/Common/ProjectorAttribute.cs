namespace Goblin.Common;

/// <summary>
/// 投影字段注解
/// 标记字段参与 Property Sync → SG 生成脏标记属性 + TakeProjectValues + 序列化
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class ProjectorAttribute : System.Attribute
{
    /// <summary>
    /// 字段索引，类内唯一，对应 fieldmask 位
    /// </summary>
    public int index { get; }

    /// <summary>
    /// Reset 时的非零缺省值（可选）
    /// </summary>
    public int defaultvalue { get; set; }

    public ProjectorAttribute(int index)
    {
        this.index = index;
    }
}

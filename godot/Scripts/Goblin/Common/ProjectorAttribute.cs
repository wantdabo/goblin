namespace Goblin.Common;

/// <summary>
/// 投影属性注解（类级，AllowMultiple）
/// 标记 BehaviorInfo 子类的需同步属性，SG 生成 backing field + 脏标记属性
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class ProjectorAttribute : System.Attribute
{
    /// <summary>
    /// 属性名
    /// </summary>
    public string name { get; }

    /// <summary>
    /// C# 类型
    /// </summary>
    public System.Type type { get; }

    /// <summary>
    /// 位索引，类内唯一，对应 projectdirtymask 位
    /// 不填时 SG 按声明顺序自动递增
    /// </summary>
    public int index { get; }

    public ProjectorAttribute(string name, System.Type type, int index = 0)
    {
        this.name = name;
        this.type = type;
        this.index = index;
    }
}

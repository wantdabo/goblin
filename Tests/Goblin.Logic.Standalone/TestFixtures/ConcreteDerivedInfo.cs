namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 6：继承抽象基类（测试 SG Clone 包含父类字段）
/// </summary>
public partial class ConcreteDerivedInfo : AbstractBaseInfo
{
    public string name { get; set; }
    public int value { get; set; }
    public bool enabled { get; set; }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Goblin.SourceGenerators;

namespace Goblin.SourceGenerators.Tests;

/// <summary>
/// T1.1 快照测试：验证 SG 扫描 partial class + IGBL → 产出 .g.cs
/// 使用 GeneratorDriver 直接调用 SG，避免包版本冲突
/// </summary>
public class GeneratorSnapshotTests
{
    /// <summary>
    /// 运行 SG 并返回产出文件名列表
    /// </summary>
    private static string[] RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // 基础运行时引用
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new GoblinSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var result = new List<string>();
        foreach (var tree in outputCompilation.SyntaxTrees)
        {
            var fileName = System.IO.Path.GetFileName(tree.FilePath);
            if (null != fileName && fileName.EndsWith(".g.cs"))
                result.Add(fileName);
        }

        return result.ToArray();
    }

    /// <summary>
    /// 最简单的 partial class + IGBL 应触发 SG 生成空 partial 类
    /// </summary>
    [Fact]
    public void PartialClass_ImplementsIGBL_GeneratesEmptyPartial()
    {
        var source = @"
namespace Goblin.Common;

public interface IGBL
{
    void Reset();
    IGBL Clone();
}

namespace TestNs;

public partial class TestInfo : Goblin.Common.IGBL
{
    public int value;

    public void Reset()
    {
        value = 0;
    }

    public Goblin.Common.IGBL Clone()
    {
        return new TestInfo { value = this.value };
    }
}
";

        var generatedFiles = RunGenerator(source);

        Assert.Contains(generatedFiles, (string f) => f.Contains("TestInfo.lifecycle.g.cs"));
    }

    /// <summary>
    /// 非 partial class 不应触发 SG
    /// </summary>
    [Fact]
    public void NonPartialClass_DoesNotGenerate()
    {
        var source = @"
namespace Goblin.Common;

public interface IGBL
{
    void Reset();
    IGBL Clone();
}

namespace TestNs;

public class NonPartialInfo : Goblin.Common.IGBL
{
    public int value;

    public void Reset()
    {
        value = 0;
    }

    public Goblin.Common.IGBL Clone()
    {
        return new NonPartialInfo { value = this.value };
    }
}
";

        var generatedFiles = RunGenerator(source);

        Assert.Empty(generatedFiles);
    }

    /// <summary>
    /// 未实现 IGBL 的 partial class 不应触发 SG
    /// </summary>
    [Fact]
    public void PartialClass_NoIGBL_DoesNotGenerate()
    {
        var source = @"
namespace Goblin.Common;

public interface IGBL
{
    void Reset();
    IGBL Clone();
}

namespace TestNs;

public partial class NoIGBLInfo
{
    public int value;
}
";

        var generatedFiles = RunGenerator(source);

        Assert.Empty(generatedFiles);
    }

    /// <summary>
    /// 通过基类继承 IGBL 的 partial class 应触发 SG
    /// </summary>
    [Fact]
    public void PartialClass_BaseImplementsIGBL_GeneratesEmptyPartial()
    {
        var source = @"
namespace Goblin.Common;

public interface IGBL
{
    void Reset();
    IGBL Clone();
}

namespace BaseNs;

public abstract class BaseInfo : Goblin.Common.IGBL
{
    public abstract void Reset();
    public abstract Goblin.Common.IGBL Clone();
}

namespace TestNs;

public partial class DerivedInfo : BaseNs.BaseInfo
{
    public int value;

    public override void Reset()
    {
        value = 0;
    }

    public override Goblin.Common.IGBL Clone()
    {
        return new DerivedInfo { value = this.value };
    }
}
";

        var generatedFiles = RunGenerator(source);

        Assert.Contains(generatedFiles, (string f) => f.Contains("DerivedInfo.lifecycle.g.cs"));
    }
}

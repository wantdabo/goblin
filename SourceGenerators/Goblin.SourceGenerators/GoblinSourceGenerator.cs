using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Goblin.SourceGenerators;

/// <summary>
/// Goblin Source Generator 入口
/// 扫描 partial class + IGBL → 产出 .g.cs
/// 扫描 [Projector] 类级注解 → 生成 backing field + 脏标记属性
/// </summary>
[Generator]
public class GoblinSourceGenerator : IIncrementalGenerator
{
    private const string IGBL_FULLNAME = "Goblin.Common.IGBL";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 管线 1：IGBL 扫描 — 需要 SemanticModel 检测接口实现（含继承链）
        var igblClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassWithBaseList(node),
                transform: static (ctx, _) => CheckIGBL(ctx));

        context.RegisterSourceOutput(
            igblClasses.Where(static data => null != data.symbol),
            static (spc, data) => EmitEmptyPartial(spc, data.symbol!, data.classDecl));

        // 管线 2：Projector 扫描 — 纯语法层，无需 SemanticModel
        var projectorData = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassWithProjector(node),
                transform: static (ctx, _) => ExtractProjectorData((ClassDeclarationSyntax)ctx.Node));

        context.RegisterSourceOutput(
            projectorData.Where(static data => null != data && data.fields.Count > 0),
            static (spc, data) => EmitProjectorCode(spc, data!));
    }

    /// <summary>
    /// 语法层过滤：partial class 且有 BaseList
    /// </summary>
    private static bool IsPartialClassWithBaseList(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl) return false;
        if (null == classDecl.BaseList) return false;

        foreach (var modifier in classDecl.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword)) return true;
        }

        return false;
    }

    /// <summary>
    /// 语法层过滤：partial class 且有 [Projector] 注解
    /// </summary>
    private static bool IsPartialClassWithProjector(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl) return false;
        if (0 == classDecl.AttributeLists.Count) return false;

        var hasPartial = false;
        foreach (var modifier in classDecl.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                hasPartial = true;
                break;
            }
        }

        if (false == hasPartial) return false;

        foreach (var attrList in classDecl.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var name = attr.Name.ToString();
                if (name == "Projector" || name == "ProjectorAttribute")
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// IGBL 语义数据
    /// </summary>
    private sealed record IgblResult(INamedTypeSymbol? symbol, ClassDeclarationSyntax classDecl);

    /// <summary>
    /// 语义层检查：是否实现 IGBL 接口（含继承链，AllInterfaces 自动追溯）
    /// </summary>
    private static IgblResult CheckIGBL(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var model = ctx.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (null == symbol) return new IgblResult(null, classDecl);

        foreach (var iface in symbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == IGBL_FULLNAME)
                return new IgblResult(symbol, classDecl);
        }

        return new IgblResult(null, classDecl);
    }

    /// <summary>
    /// Projector 字段数据
    /// </summary>
    private sealed record ProjectionFieldData(
        string name,
        string typeText,
        int index,
        int defaultvalue,
        string? summary
    );

    /// <summary>
    /// Projector 类数据
    /// </summary>
    private sealed record ProjectionClassData(
        string ns,
        string className,
        List<ProjectionFieldData> fields
    );

    /// <summary>
    /// 从语法节点提取 [Projector] 数据
    /// </summary>
    private static ProjectionClassData? ExtractProjectorData(ClassDeclarationSyntax classDecl)
    {
        // 获取命名空间
        var ns = GetNamespace(classDecl);

        var fields = new List<ProjectionFieldData>();
        var attributeLists = classDecl.AttributeLists;

        if (0 == attributeLists.Count) return null;

        foreach (var attrList in attributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                // 匹配 Projector 或 ProjectorAttribute
                if (false == (attrName == "Projector" || attrName == "ProjectorAttribute")) continue;

                var args = attr.ArgumentList?.Arguments;
                if (false == args.HasValue || args.Value.Count < 3) continue;

                // 提取 name（第一个参数，字符串字面量）
                var name = ExtractStringArg(args.Value[0]);
                if (null == name) continue;

                // 提取 type（第二个参数，typeof(...) 表达式）
                var typeText = ExtractTypeOfArg(args.Value[1]);
                if (null == typeText) continue;

                // 提取 index（第三个参数，整数）
                var index = ExtractIntArg(args.Value[2]);
                if (null == index) continue;

                // 提取可选 defaultvalue
                var defaultvalue = 0;
                foreach (var arg in args.Value)
                {
                    if (arg.NameEquals?.Name.Identifier.Text == "defaultvalue")
                    {
                        defaultvalue = ExtractIntArg(arg) ?? 0;
                    }
                }

                // 提取摘要注释
                var summary = ExtractLeadingComment(attrList);

                fields.Add(new ProjectionFieldData(name, typeText, index.Value, defaultvalue, summary));
            }
        }

        if (0 == fields.Count) return null;

        return new ProjectionClassData(ns, classDecl.Identifier.Text, fields);
    }

    /// <summary>
    /// 产出 IGBL 空 partial 类
    /// </summary>
    private static void EmitEmptyPartial(SourceProductionContext context, INamedTypeSymbol symbol, ClassDeclarationSyntax classDecl)
    {
        var namespaceName = symbol.ContainingNamespace.ToDisplayString();
        var className = symbol.Name;

        var source = $@"// <auto-generated/>
namespace {namespaceName};

partial class {className}
{{
}}
";
        var hintName = $"{namespaceName}.{className}.g.cs";
        context.AddSource(hintName, source);
    }

    /// <summary>
    /// 产出 Projector 脏标记属性 + backing field
    /// </summary>
    private static void EmitProjectorCode(SourceProductionContext context, ProjectionClassData data)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine($"namespace {data.ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {data.className}");
        sb.AppendLine("{");

        // Backing fields
        var classNameLower = data.className.ToLowerInvariant();
        foreach (var field in data.fields)
        {
            sb.AppendLine($"    private {field.typeText} {classNameLower}_{field.name} {{ get; set; }}");
        }

        sb.AppendLine();

        // Properties with dirty tracking
        foreach (var field in data.fields)
        {
            if (null != field.summary)
            {
                sb.AppendLine("    /// <summary>");
                sb.AppendLine($"    /// {field.summary}");
                sb.AppendLine("    /// </summary>");
            }
            sb.AppendLine($"    public {field.typeText} {field.name}");
            sb.AppendLine("    {");
            sb.AppendLine($"        get => {classNameLower}_{field.name};");
            sb.AppendLine("        set");
            sb.AppendLine("        {");
            sb.AppendLine($"            if ({classNameLower}_{field.name} != value)");
            sb.AppendLine("            {");
            sb.AppendLine($"                {classNameLower}_{field.name} = value;");
            sb.AppendLine($"                projectdirtymask |= 1ul << {field.index};");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        // TakeProjectValues
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 按脏标记掩码收集值到 object[]");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public object[] TakeProjectValues(ulong mask)");
        sb.AppendLine("    {");
        sb.AppendLine("        var list = new System.Collections.Generic.List<object>();");
        foreach (var field in data.fields)
        {
            sb.AppendLine($"        if (0ul != (mask & (1ul << {field.index})))");
            sb.AppendLine($"            list.Add({classNameLower}_{field.name});");
        }
        sb.AppendLine("        return list.ToArray();");
        sb.AppendLine("    }");

        sb.AppendLine();

        // ClearProjectDirty
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 清除脏标记");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public void ClearProjectDirty()");
        sb.AppendLine("    {");
        sb.AppendLine("        projectdirtymask = 0;");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        var hintName = $"{data.ns}.{data.className}.projector.g.cs";
        context.AddSource(hintName, sb.ToString());
    }

    /// <summary>
    /// 获取类的命名空间
    /// </summary>
    private static string GetNamespace(ClassDeclarationSyntax classDecl)
    {
        var parent = classDecl.Parent;
        while (null != parent)
        {
            if (parent is BaseNamespaceDeclarationSyntax ns)
                return ns.Name.ToString();
            if (parent is FileScopedNamespaceDeclarationSyntax fns)
                return fns.Name.ToString();
            parent = parent.Parent;
        }
        return string.Empty;
    }

    /// <summary>
    /// 提取字符串字面量参数值
    /// </summary>
    private static string? ExtractStringArg(AttributeArgumentSyntax arg)
    {
        var expr = arg.Expression;
        if (expr is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            var text = literal.Token.ValueText;
            return text;
        }
        return null;
    }

    /// <summary>
    /// 提取 typeof(...) 中的类型名
    /// </summary>
    private static string? ExtractTypeOfArg(AttributeArgumentSyntax arg)
    {
        var expr = arg.Expression;
        if (expr is TypeOfExpressionSyntax typeOf
            && typeOf.Type is IdentifierNameSyntax id)
        {
            return id.Identifier.Text;
        }
        // 含点号的类型名，如 Kowtow.Math.FPVector3
        if (expr is TypeOfExpressionSyntax typeOfQualified
            && typeOfQualified.Type is QualifiedNameSyntax qn)
        {
            return qn.ToString();
        }
        return null;
    }

    /// <summary>
    /// 提取整数参数值
    /// </summary>
    private static int? ExtractIntArg(AttributeArgumentSyntax arg)
    {
        var expr = arg.Expression;
        if (expr is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            if (int.TryParse(literal.Token.ValueText, out var val))
                return val;
        }
        return null;
    }

    /// <summary>
    /// 从 AttributeList 的 leading trivia 提取最近一行 // 注释
    /// </summary>
    private static string? ExtractLeadingComment(AttributeListSyntax attrList)
    {
        var trivia = attrList.GetLeadingTrivia();
        // 从后往前找最后一个非空格/非换行的单行注释
        for (var i = trivia.Count - 1; i >= 0; i--)
        {
            var t = trivia[i];
            if (t.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                var text = t.ToString().TrimStart('/').Trim();
                if (false == string.IsNullOrEmpty(text))
                    return text;
            }
            // 遇到非空格非换行就停（已经到上一个代码了）
            if (false == t.IsKind(SyntaxKind.WhitespaceTrivia)
                && false == t.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                break;
            }
        }
        return null;
    }
}

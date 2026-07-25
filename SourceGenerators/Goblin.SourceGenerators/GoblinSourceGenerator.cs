using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Goblin.SourceGenerators;

/// <summary>
/// Goblin Source Generator 入口
/// 扫描 partial class + IGBL → 产出 override Reset / Clone
/// 扫描 [Projector] 类级注解 → 生成 backing field + 脏标记属性
/// </summary>
[Generator]
public class GoblinSourceGenerator : IIncrementalGenerator
{
    private const string IGBL_FULLNAME = "Goblin.Common.IGBL";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 管线 1：IGBL 扫描 — 需要 SemanticModel 做字段分类
        var igblClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassWithBaseList(node),
                transform: static (ctx, _) => ExtractLifecycleData(ctx));

        context.RegisterSourceOutput(
            igblClasses.Where(static data => null != data && data.symbol != null),
            static (spc, data) => EmitLifecycleCode(spc, data));

        // 管线 2：Projector 扫描 — 纯语法层，无需 SemanticModel
        var projectorData = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassWithProjector(node),
                transform: static (ctx, _) => ExtractProjectorData((ClassDeclarationSyntax)ctx.Node));

        context.RegisterSourceOutput(
            projectorData.Where(static data => null != data && 0 < data.fields.Count),
            static (spc, data) => EmitProjectorCode(spc, data!));
    }

    // ============================================================
    // 语法层过滤
    // ============================================================

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
                if ("Projector" == name || "ProjectorAttribute" == name)
                    return true;
            }
        }

        return false;
    }

    // ============================================================
    // 数据模型
    // ============================================================

    /// <summary>
    /// 字段分类
    /// </summary>
    private enum FieldCategory
    {
        ValueType,
        IGBL,
        ContainerValue,
        ContainerIGBL,
        ContainerNestedValue,
        ContainerNestedIGBL,
        Reference
    }

    /// <summary>
    /// 生命周期字段数据
    /// </summary>
    private sealed record LifecycleFieldData(
        string name,
        string typeName,
        FieldCategory category,
        string? elementType,
        string? innerTypeName,
        string? innerElementTypeName
    );

    /// <summary>
    /// 生命周期类数据（IGBL 管线输出）
    /// </summary>
    private sealed record LifecycleClassData(
        INamedTypeSymbol? symbol,
        string ns,
        string className,
        string classNameLower,
        List<LifecycleFieldData> fields,
        List<ProjectionFieldData> projectorFields,
        bool isBehaviorInfo
    );

    /// <summary>
    /// Projector 字段数据（两条管线共用）
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
        List<ProjectionFieldData> fields,
        List<string> usings
    );

    // ============================================================
    // 管线 1：IGBL 生命周期数据提取
    // ============================================================

    /// <summary>
    /// 从语义上下文提取生命周期类数据
    /// 检查 IGBL 接口、分类所有字段、收集 Projector 注解
    /// </summary>
    private static LifecycleClassData ExtractLifecycleData(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var model = ctx.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (null == symbol) return null!;

        // 检查是否实现 IGBL
        var hasIGBL = false;
        foreach (var iface in symbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == IGBL_FULLNAME)
            {
                hasIGBL = true;
                break;
            }
        }

        if (false == hasIGBL) return null!;

        // 检查是否继承 BehaviorInfo（决定生成 override 还是接口实现）
        var isBehaviorInfo = false;
        var baseType = symbol.BaseType;
        while (null != baseType)
        {
            if (baseType.ToDisplayString() == "Goblin.Gameplay.Logic.Core.BehaviorInfo")
            {
                isBehaviorInfo = true;
                break;
            }
            baseType = baseType.BaseType;
        }

        var ns = symbol.ContainingNamespace.ToDisplayString();
        var className = symbol.Name;
        var classNameLower = className.ToLowerInvariant();

        // 收集并分类该类声明的属性
        var fields = new List<LifecycleFieldData>();
        var members = symbol.GetMembers();
        foreach (var member in members)
        {
            if (member is not IPropertySymbol prop) continue;
            // 跳过基类/静态/索引器
            if (prop.IsStatic) continue;
            if (prop.IsIndexer) continue;
            // 只纳入有 setter 的属性
            if (null == prop.SetMethod) continue;
            // 检查是否是当前类声明的（非继承）
            if (false == SymbolEqualityComparer.Default.Equals(prop.ContainingType, symbol)) continue;

            var type = prop.Type;
            var category = ClassifyType(type);
            string? elementType = null;
            string? innerTypeName = null;
            string? innerElementTypeName = null;

            if (category == FieldCategory.ContainerValue || category == FieldCategory.ContainerIGBL)
            {
                elementType = GetContainerElementType(type);
            }
            else if (category == FieldCategory.ContainerNestedValue || category == FieldCategory.ContainerNestedIGBL)
            {
                // 嵌套容器：外层容器的元素是内层容器
                if (type is INamedTypeSymbol outer && outer.TypeArguments.Length > 0)
                {
                    var inner = outer.TypeArguments[outer.TypeArguments.Length - 1];
                    innerTypeName = inner.ToDisplayString();
                    if (inner is INamedTypeSymbol innerNamed && innerNamed.TypeArguments.Length > 0)
                    {
                        innerElementTypeName = innerNamed.TypeArguments[innerNamed.TypeArguments.Length - 1].ToDisplayString();
                    }
                }
            }

            fields.Add(new LifecycleFieldData(
                prop.Name,
                prop.Type.ToDisplayString(),
                category,
                elementType,
                innerTypeName,
                innerElementTypeName
            ));
        }

        // 提取 Projector 注解（用于 Reset 中重置 backing field）
        var projectorFields = new List<ProjectionFieldData>();
        var attributeLists = classDecl.AttributeLists;
        foreach (var attrList in attributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (false == ("Projector" == attrName || "ProjectorAttribute" == attrName)) continue;

                var args = attr.ArgumentList?.Arguments;
                if (false == args.HasValue || args.Value.Count < 3) continue;

                var name = ExtractStringArg(args.Value[0]);
                if (null == name) continue;

                var typeText = ExtractTypeOfArg(args.Value[1]);
                if (null == typeText) continue;

                var index = ExtractIntArg(args.Value[2]);
                if (null == index) continue;

                var defaultvalue = 0;
                foreach (var arg in args.Value)
                {
                    if ("defaultvalue" == arg.NameEquals?.Name.Identifier.Text)
                    {
                        defaultvalue = ExtractIntArg(arg) ?? 0;
                    }
                }

                projectorFields.Add(new ProjectionFieldData(name, typeText, index.Value, defaultvalue, null));
            }
        }

        return new LifecycleClassData(
            symbol,
            ns,
            className,
            classNameLower,
            fields,
            projectorFields,
            isBehaviorInfo
        );
    }

    /// <summary>
    /// 分类字段类型
    /// </summary>
    private static FieldCategory ClassifyType(ITypeSymbol type)
    {
        // 值类型
        if (type.IsValueType) return FieldCategory.ValueType;

        // string 当值类型处理
        if (type.SpecialType == SpecialType.System_String) return FieldCategory.ValueType;

        // IGBL 引用
        if (IsIGBL(type)) return FieldCategory.IGBL;

        // 容器检查
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var originalName = namedType.OriginalDefinition.ToDisplayString();
            if (IsKnownContainer(originalName))
            {
                // 取元素类型（最后 TypeArgument，Dictionary 取 Value）
                var elementType = GetLastTypeArgument(namedType);
                if (null == elementType) return FieldCategory.ContainerValue;

                // 元素是 IGBL
                if (IsIGBL(elementType)) return FieldCategory.ContainerIGBL;

                // 元素是容器（嵌套）
                if (IsContainerType(elementType))
                {
                    var innerElement = GetLastTypeArgument((INamedTypeSymbol)elementType);
                    if (null != innerElement && IsIGBL(innerElement)) return FieldCategory.ContainerNestedIGBL;
                    return FieldCategory.ContainerNestedValue;
                }

                return FieldCategory.ContainerValue;
            }
        }

        // 其他引用类型
        return FieldCategory.Reference;
    }

    /// <summary>
    /// 判断类型是否实现 IGBL
    /// </summary>
    private static bool IsIGBL(ITypeSymbol type)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.ToDisplayString() == IGBL_FULLNAME) return true;
        }
        return false;
    }

    /// <summary>
    /// 判断类型是否为已知容器
    /// </summary>
    private static bool IsContainerType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            return IsKnownContainer(namedType.OriginalDefinition.ToDisplayString());
        }
        return false;
    }

    /// <summary>
    /// 获取最后一个类型参数
    /// </summary>
    private static ITypeSymbol? GetLastTypeArgument(INamedTypeSymbol namedType)
    {
        if (namedType.TypeArguments.Length > 0)
            return namedType.TypeArguments[namedType.TypeArguments.Length - 1];
        return null;
    }

    /// <summary>
    /// 判断是否为已知容器类型
    /// </summary>
    private static bool IsKnownContainer(string originalDefinitionName)
    {
        return originalDefinitionName == "System.Collections.Generic.List<T>"
            || originalDefinitionName == "System.Collections.Generic.Dictionary<TKey, TValue>"
            || originalDefinitionName == "System.Collections.Generic.HashSet<T>"
            || originalDefinitionName == "System.Collections.Generic.Queue<T>"
            || originalDefinitionName == "System.Collections.Generic.Stack<T>";
    }

    /// <summary>
    /// 获取容器元素类型名（取最后一个类型参数，适用于单值容器和 Dictionary）
    /// </summary>
    private static string? GetContainerElementType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
        {
            var lastArg = namedType.TypeArguments[namedType.TypeArguments.Length - 1];
            return lastArg.ToDisplayString();
        }
        return null;
    }

    // ============================================================
    // 管线 1 输出：生命周期代码生成
    // ============================================================

    private static void EmitLifecycleCode(SourceProductionContext context, LifecycleClassData data)
    {
        var ns = data.ns;
        var className = data.className;

        var sb = new System.Text.StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine($"using Goblin.Common;");
        if (data.isBehaviorInfo)
        {
            sb.AppendLine($"using Goblin.Gameplay.Logic.Common;");
            sb.AppendLine($"using Goblin.Gameplay.Logic.Core;");
        }
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {className}");
        sb.AppendLine("{");

        // ---- Reset() ----
        EmitReset(sb, data);

        // ---- Clone() ----
        EmitClone(sb, data);

        sb.AppendLine("}");

        var hintName = $"{ns}.{className}.lifecycle.g.cs";
        context.AddSource(hintName, sb.ToString());
    }

    private static void EmitReset(System.Text.StringBuilder sb, LifecycleClassData data)
    {
        var modifier = data.isBehaviorInfo ? "public override" : "public";

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 重置对象状态，回收前调用");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    {modifier} void Reset()");
        sb.AppendLine("    {");

        // 容器 IGBL 字段：先回收元素再清空
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerIGBL) continue;
            sb.AppendLine($"        if (null != {field.name})");
            sb.AppendLine("        {");
            sb.AppendLine($"            foreach (var item in {field.name})");
            sb.AppendLine("            {");
            sb.AppendLine("                item.Reset();");
            sb.AppendLine("                ObjectCache.Set(item);");
            sb.AppendLine("            }");
            sb.AppendLine($"            {field.name}.Clear();");
            sb.AppendLine("        }");
        }

        // 容器值类型字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerValue) continue;
            sb.AppendLine($"        {field.name}?.Clear();");
        }

        // 嵌套容器字段：外层遍历内层 Clear，外层 Clear（容器只清不还）
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerNestedValue && field.category != FieldCategory.ContainerNestedIGBL) continue;
            sb.AppendLine($"        if (null != {field.name})");
            sb.AppendLine("        {");
            sb.AppendLine($"            foreach (var kv in {field.name}) kv.Value?.Clear();");
            sb.AppendLine($"            {field.name}.Clear();");
            sb.AppendLine("        }");
        }

        // IGBL 引用字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.IGBL) continue;
            sb.AppendLine($"        {field.name}?.Reset();");
            sb.AppendLine($"        ObjectCache.Set({field.name});");
            sb.AppendLine($"        {field.name} = null;");
        }

        // 值类型字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ValueType) continue;
            sb.AppendLine($"        {field.name} = default;");
        }

        // 引用字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.Reference) continue;
            sb.AppendLine($"        {field.name} = null;");
        }

        // Projector backing field 重置
        foreach (var pf in data.projectorFields)
        {
            if (HasImplicitIntConversion(pf.typeText))
                sb.AppendLine($"        {data.classNameLower}_{pf.name} = {pf.defaultvalue};");
            else
                sb.AppendLine($"        {data.classNameLower}_{pf.name} = default;");
        }

        // 脏标记清零
        if (0 < data.projectorFields.Count)
        {
            sb.AppendLine("        projectdirtymask = 0;");
        }

        // 尾调基类（仅 BehaviorInfo 子类）
        if (data.isBehaviorInfo)
        {
            sb.AppendLine("        base.Reset();");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitClone(System.Text.StringBuilder sb, LifecycleClassData data)
    {
        var className = data.className;
        var returnType = data.isBehaviorInfo ? "BehaviorInfo" : "IGBL";
        var modifier = data.isBehaviorInfo ? "public override" : "public";

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 深度克隆");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    {modifier} {returnType} Clone()");
        sb.AppendLine("    {");

        sb.AppendLine($"        var c = ObjectCache.Ensure<{className}>();");
        // Ready 先于字段拷贝：OnReady/OnReset 初始化，随后字段拷贝覆盖为目标值（仅 BehaviorInfo 子类）
        if (data.isBehaviorInfo)
        {
            sb.AppendLine("        c.Ready(actor);");
        }

        // 值类型字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ValueType) continue;
            sb.AppendLine($"        c.{field.name} = this.{field.name};");
        }

        // 引用字段（浅拷贝）
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.Reference) continue;
            sb.AppendLine($"        c.{field.name} = this.{field.name};");
        }

        // 容器值类型字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerValue) continue;
            sb.AppendLine($"        c.{field.name} = null != this.{field.name} ? new {field.typeName}(this.{field.name}) : null;");
        }

        // 嵌套容器字段（内层值类型）：遍历外层，内层 new 拷贝
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerNestedValue) continue;
            sb.AppendLine($"        if (null != this.{field.name})");
            sb.AppendLine("        {");
            sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
            sb.AppendLine($"            foreach (var kv in this.{field.name})");
            sb.AppendLine($"                c.{field.name}.Add(kv.Key, new {field.innerTypeName}(kv.Value));");
            sb.AppendLine("        }");
        }

        // 嵌套容器字段（内层 IGBL）：遍历外层，内层 new + foreach Clone
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerNestedIGBL) continue;
            sb.AppendLine($"        if (null != this.{field.name})");
            sb.AppendLine("        {");
            sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
            sb.AppendLine($"            foreach (var kv in this.{field.name})");
            sb.AppendLine("            {");
            sb.AppendLine($"                var inner = new {field.innerTypeName}(kv.Value.Count);");
            sb.AppendLine($"                foreach (var item in kv.Value) inner.Add(({field.innerElementTypeName})item.Clone());");
            sb.AppendLine($"                c.{field.name}.Add(kv.Key, inner);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        // 容器 IGBL 字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerIGBL) continue;

            sb.AppendLine($"        if (null != this.{field.name})");
            sb.AppendLine("        {");
            sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
            sb.AppendLine($"            foreach (var item in this.{field.name})");
            sb.AppendLine($"                c.{field.name}.Add(({field.elementType})item.Clone());");
            sb.AppendLine("        }");
        }

        // IGBL 引用字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.IGBL) continue;
            sb.AppendLine($"        c.{field.name} = ({field.typeName})this.{field.name}?.Clone();");
        }

        sb.AppendLine("        return c;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    // ============================================================
    // 管线 2：Projector 代码生成
    // ============================================================

    private static ProjectionClassData? ExtractProjectorData(ClassDeclarationSyntax classDecl)
    {
        var ns = GetNamespace(classDecl);

        var fields = new List<ProjectionFieldData>();
        var attributeLists = classDecl.AttributeLists;

        if (0 == attributeLists.Count) return null;

        foreach (var attrList in attributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (false == ("Projector" == attrName || "ProjectorAttribute" == attrName)) continue;

                var args = attr.ArgumentList?.Arguments;
                if (false == args.HasValue || args.Value.Count < 3) continue;

                var name = ExtractStringArg(args.Value[0]);
                if (null == name) continue;

                var typeText = ExtractTypeOfArg(args.Value[1]);
                if (null == typeText) continue;

                var index = ExtractIntArg(args.Value[2]);
                if (null == index) continue;

                var defaultvalue = 0;
                foreach (var arg in args.Value)
                {
                    if ("defaultvalue" == arg.NameEquals?.Name.Identifier.Text)
                    {
                        defaultvalue = ExtractIntArg(arg) ?? 0;
                    }
                }

                var summary = ExtractLeadingComment(attrList);

                fields.Add(new ProjectionFieldData(name, typeText, index.Value, defaultvalue, summary));
            }
        }

        if (0 == fields.Count) return null;

        var usings = CollectUsings(classDecl);
        return new ProjectionClassData(ns, classDecl.Identifier.Text, fields, usings);
    }

    private static void EmitProjectorCode(SourceProductionContext context, ProjectionClassData data)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        foreach (var u in data.usings)
        {
            sb.AppendLine(u);
        }
        sb.AppendLine("using Goblin.Gameplay.Projection;");
        sb.AppendLine();
        sb.AppendLine($"namespace {data.ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {data.className} : IProjectable");
        sb.AppendLine("{");

        var classNameLower = data.className.ToLowerInvariant();
        foreach (var field in data.fields)
        {
            sb.AppendLine($"    private {field.typeText} {classNameLower}_{field.name} {{ get; set; }}");
        }

        sb.AppendLine();

        // IProjectable 实现：投影脏标记
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 投影脏标记，位图对应 [Projector(index)] 字段");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public ulong projectdirtymask { get; set; }");
        sb.AppendLine();

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

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 清除脏标记");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public void ClearProjectDirty()");
        sb.AppendLine("    {");
        sb.AppendLine("        projectdirtymask = 0;");
        sb.AppendLine("    }");

        sb.AppendLine();

        // 全量脏标记（新对象首帧全量同步）
        var allMaskExpr = 0 == data.fields.Count
            ? "0ul"
            : string.Join(" | ", data.fields.Select(f => $"(1ul << {f.index})"));
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 标记全部投影字段为脏（新对象全量同步）");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public void MarkAllDirty()");
        sb.AppendLine("    {");
        sb.AppendLine($"        projectdirtymask = {allMaskExpr};");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        var hintName = $"{data.ns}.{data.className}.projector.g.cs";
        context.AddSource(hintName, sb.ToString());
    }

    private static bool HasImplicitIntConversion(string typeText)
    {
        return "int" == typeText
            || "long" == typeText
            || "float" == typeText
            || "double" == typeText
            || "FP" == typeText;
    }

    // ============================================================
    // 共用工具方法
    // ============================================================

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
    /// 收集源文件的 using 指令，用于生成代码中引用外部命名空间
    /// </summary>
    private static List<string> CollectUsings(ClassDeclarationSyntax classDecl)
    {
        var usings = new List<string>();
        var root = classDecl.SyntaxTree.GetRoot();
        foreach (var node in root.DescendantNodes())
        {
            if (node is UsingDirectiveSyntax usingDir)
            {
                usings.Add(usingDir.ToString());
            }
        }
        return usings;
    }

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

    private static string? ExtractTypeOfArg(AttributeArgumentSyntax arg)
    {
        var expr = arg.Expression;
        if (expr is TypeOfExpressionSyntax typeOf
            && typeOf.Type is IdentifierNameSyntax id)
        {
            return id.Identifier.Text;
        }
        if (expr is TypeOfExpressionSyntax typeOfQualified
            && typeOfQualified.Type is QualifiedNameSyntax qn)
        {
            return qn.ToString();
        }
        return null;
    }

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

    private static string? ExtractLeadingComment(AttributeListSyntax attrList)
    {
        var trivia = attrList.GetLeadingTrivia();
        for (var i = trivia.Count - 1; i >= 0; i--)
        {
            var t = trivia[i];
            if (t.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                var text = t.ToString().TrimStart('/').Trim();
                if (false == string.IsNullOrEmpty(text))
                    return text;
            }
            if (false == t.IsKind(SyntaxKind.WhitespaceTrivia)
                && false == t.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                break;
            }
        }
        return null;
    }
}

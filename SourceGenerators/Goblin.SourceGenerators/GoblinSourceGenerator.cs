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

        // 管线 3：Component ApplyTo 生成 — 扫描 [ProjectorTarget] 注解
        var applyToData = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassWithProjectorTarget(node),
                transform: static (ctx, _) => ExtractApplyToData(ctx));

        context.RegisterSourceOutput(
            applyToData.Where(static data => null != data),
            static (spc, data) => EmitApplyToCode(spc, data));
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
        bool isBehaviorInfo,
        bool isAbstract,
        bool hasOnReady,
        bool hasOnReset,
        bool hasOnClone,
        List<LifecycleFieldData>? parentFields
    );

    /// <summary>
    /// Projector 字段数据（两条管线共用）
    /// </summary>
    private sealed record ProjectionFieldData(
        string name,
        string typeText,
        int index,
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

        var isAbstract = symbol.IsAbstract;

        // 检测该类是否已有 SGReady 重写（非继承）
        var sgReadyMethod = isBehaviorInfo ? "SGReady" : "OnReady";
        var hasOnReady = false;
        foreach (var member in symbol.GetMembers(sgReadyMethod))
        {
            if (member is IMethodSymbol m
                && SymbolEqualityComparer.Default.Equals(m.ContainingType, symbol)
                && false == m.IsStatic)
            {
                hasOnReady = true;
                break;
            }
        }

        // 检测该类是否已有 SGReset 或 Reset 重写
        var sgResetMethod = isBehaviorInfo ? "SGReset" : "Reset";
        var hasOnReset = false;
        foreach (var member in symbol.GetMembers(sgResetMethod))
        {
            if (member is IMethodSymbol m
                && SymbolEqualityComparer.Default.Equals(m.ContainingType, symbol)
                && false == m.IsStatic)
            {
                hasOnReset = true;
                break;
            }
        }

        // 检测该类是否已有 SGClone 或 Clone 重写
        var sgCloneMethod = isBehaviorInfo ? "SGClone" : "Clone";
        var hasOnClone = false;
        foreach (var member in symbol.GetMembers(sgCloneMethod))
        {
            if (member is IMethodSymbol m
                && SymbolEqualityComparer.Default.Equals(m.ContainingType, symbol)
                && false == m.IsStatic)
            {
                hasOnClone = true;
                break;
            }
        }

        // 收集父类字段（用于子类 Clone 深拷贝父类非 BehaviorInfo 的字段）
        var parentFields = new List<LifecycleFieldData>();
        if (isBehaviorInfo && null != symbol.BaseType)
        {
            CollectParentFields(symbol.BaseType, parentFields);
        }

        // 提取 Projector 注解（用于 Reset 中重置 backing field）
        var projectorFields = new List<ProjectionFieldData>();
        var nextAutoIndex = 0;
        var attributeLists = classDecl.AttributeLists;
        foreach (var attrList in attributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (false == ("Projector" == attrName || "ProjectorAttribute" == attrName)) continue;

                var args = attr.ArgumentList?.Arguments;
                if (false == args.HasValue || args.Value.Count < 2) continue;

                var name = ExtractStringArg(args.Value[0]);
                if (null == name) continue;

                var typeText = ExtractTypeOfArg(args.Value[1]);
                if (null == typeText) continue;

                // index 可选（第三个非命名参数），不填时自动递增
                int? index = null;
                if (args.Value.Count >= 3 && null == args.Value[2].NameEquals)
                    index = ExtractIntArg(args.Value[2]);
                if (null == index) index = nextAutoIndex;
                nextAutoIndex = index.Value + 1;

                projectorFields.Add(new ProjectionFieldData(name, typeText, index.Value, null));
            }
        }

        return new LifecycleClassData(
            symbol,
            ns,
            className,
            classNameLower,
            fields,
            projectorFields,
            isBehaviorInfo,
            isAbstract,
            hasOnReady,
            hasOnReset,
            hasOnClone,
            parentFields
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

        // 容器检查（必须在 IGBL 检查之前，因为 GBL 容器同时实现 IGBL 接口）
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

        // IGBL 引用（非容器的纯 IGBL 类型）
        if (IsIGBL(type)) return FieldCategory.IGBL;

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
            || originalDefinitionName == "System.Collections.Generic.Stack<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.GBLList<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.GBLDict<K, V>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.GBLHashSet<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.GBLQueue<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.GBLStack<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.TGBLList<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.TGBLDict<K, V>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.TGBLHashSet<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.TGBLQueue<T>"
            || originalDefinitionName == "Goblin.Gameplay.Logic.Common.TGBLStack<T>";
    }

    /// <summary>
    /// 判断类型名是否为 GBL 容器（GBLList, GBLDict, GBLHashSet, TGBLList, TGBLDict, TGBLHashSet）
    /// </summary>
    private static bool IsGBLContainerTypeName(string typeName)
    {
        return typeName.Contains("GBLList<")
            || typeName.Contains("GBLDict<")
            || typeName.Contains("GBLHashSet<")
            || typeName.Contains("TGBLList<")
            || typeName.Contains("TGBLDict<")
            || typeName.Contains("TGBLHashSet<");
    }

    /// <summary>
    /// 从全限定类型名中提取短名（去掉命名空间），例如 Goblin.Common.GBLList<int> → GBLList<int>
    /// </summary>
    private static string GetShortTypeName(string fullName)
    {
        // 找到最后一个 . 在第一个 < 之前
        var angleIdx = fullName.IndexOf('<');
        var searchEnd = angleIdx >= 0 ? angleIdx : fullName.Length;
        var lastDot = fullName.LastIndexOf('.', searchEnd - 1);
        if (lastDot >= 0) return fullName.Substring(lastDot + 1);
        return fullName;
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

    /// <summary>
    /// 收集父类链中的生命周期字段（用于子类 Clone 深拷贝父类非 BehaviorInfo 字段）
    /// 从直接基类向 BehaviorInfo 方向遍历，遇到 BehaviorInfo 本身或非 IGBL 类停止
    /// </summary>
    private static void CollectParentFields(INamedTypeSymbol? baseType, List<LifecycleFieldData> parentFields)
    {
        while (null != baseType)
        {
            var baseName = baseType.ToDisplayString();
            // 遇 BehaviorInfo 本身停止
            if ("Goblin.Gameplay.Logic.Core.BehaviorInfo" == baseName) break;

            // 检查是否实现 IGBL
            var implementsIGBL = false;
            foreach (var iface in baseType.AllInterfaces)
            {
                if (iface.ToDisplayString() == IGBL_FULLNAME)
                {
                    implementsIGBL = true;
                    break;
                }
            }
            if (false == implementsIGBL) break;

            // 收集该基类的自有属性字段
            var members = baseType.GetMembers();
            foreach (var member in members)
            {
                if (member is not IPropertySymbol prop) continue;
                if (prop.IsStatic || prop.IsIndexer) continue;
                if (null == prop.SetMethod) continue;
                // 只纳入该基类声明的（非更深层继承）
                if (false == SymbolEqualityComparer.Default.Equals(prop.ContainingType, baseType)) continue;

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

                parentFields.Add(new LifecycleFieldData(
                    prop.Name,
                    prop.Type.ToDisplayString(),
                    category,
                    elementType,
                    innerTypeName,
                    innerElementTypeName
                ));
            }

            baseType = baseType.BaseType;
        }
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
        sb.AppendLine($"using Goblin.Gameplay.Logic.Common;");
        if (data.isBehaviorInfo)
        {
            sb.AppendLine($"using Goblin.Gameplay.Logic.Core;");
        }
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {className}");
        sb.AppendLine("{");

        if (data.isBehaviorInfo)
        {
            // ---- SGReady() ----
            // BehaviorInfo 子类且用户未手动定义时生成
            if (false == data.hasOnReady)
            {
                EmitSGReady(sb, data);
            }

            // ---- SGReset() ----
            if (false == data.hasOnReset)
            {
                EmitReset(sb, data);
            }

            // ---- SGClone() ----
            if (false == data.isAbstract && false == data.hasOnClone)
            {
                EmitClone(sb, data);
            }
        }
        else
        {
            // ---- Reset() ----
            if (false == data.hasOnReset)
            {
                EmitReset(sb, data);
            }

            // ---- Clone() ----
            if (false == data.isAbstract && false == data.hasOnClone)
            {
                EmitClone(sb, data);
            }
        }

        sb.AppendLine("}");

        var hintName = $"{ns}.{className}.lifecycle.g.cs";
        context.AddSource(hintName, sb.ToString());
    }

    /// <summary>
    /// 判断类型名是否为 GBL 容器（自带元素生命周期管理）
    /// </summary>
    private static bool IsGBLContainer(string typeName)
    {
        return typeName.StartsWith("Goblin.Gameplay.Logic.Common.GBLDict<")
            || typeName.StartsWith("Goblin.Gameplay.Logic.Common.GBLList<")
            || typeName.StartsWith("Goblin.Gameplay.Logic.Common.GBLHashSet<")
            || typeName.StartsWith("Goblin.Gameplay.Logic.Common.TGBLDict<")
            || typeName.StartsWith("Goblin.Gameplay.Logic.Common.TGBLList<");
    }

    /// <summary>
    /// 生成 SGReady 重写，初始化所有容器字段
    /// </summary>
    private static void EmitSGReady(System.Text.StringBuilder sb, LifecycleClassData data)
    {
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// SG 生成的 Ready 初始化，仅初始化容器字段");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    protected override void SGReady()");
        sb.AppendLine("    {");

        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerValue
                && field.category != FieldCategory.ContainerIGBL
                && field.category != FieldCategory.ContainerNestedValue
                && field.category != FieldCategory.ContainerNestedIGBL)
                continue;

            if (IsGBLContainer(field.typeName))
                sb.AppendLine($"        if (null == {field.name}) {field.name} = ObjectCache.Ensure<{field.typeName}>();");
            else
                sb.AppendLine($"        if (null == {field.name}) {field.name} = new {field.typeName}();");
        }

        sb.AppendLine("        base.SGReady();");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitReset(System.Text.StringBuilder sb, LifecycleClassData data)
    {
        var methodName = data.isBehaviorInfo ? "SGReset" : "Reset";
        var modifier = data.isBehaviorInfo ? "protected override" : "public";

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 重置对象状态，回收前调用");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    {modifier} void {methodName}()");
        sb.AppendLine("    {");

        // 容器 IGBL 字段：先回收元素再清空
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerIGBL) continue;
            // GBL 容器：元素生命周期由容器管理，容器本身归还池
            if (IsGBLContainer(field.typeName))
            {
                sb.AppendLine($"        {field.name}?.Reset();");
                sb.AppendLine($"        ObjectCache.Set({field.name});");
                sb.AppendLine($"        {field.name} = null;");
            }
            else
            {
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
        }

        // 容器值类型字段
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerValue) continue;
            if (IsGBLContainer(field.typeName))
            {
                sb.AppendLine($"        {field.name}?.Reset();");
                sb.AppendLine($"        ObjectCache.Set({field.name});");
                sb.AppendLine($"        {field.name} = null;");
            }
            else
            {
                sb.AppendLine($"        {field.name}?.Clear();");
            }
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
            var resetValue = NeedsInit(pf.typeText) ? $"new {pf.typeText}()" : "default";
            sb.AppendLine($"        {data.classNameLower}_{pf.name} = {resetValue};");
        }

        // 脏标记清零
        if (0 < data.projectorFields.Count)
        {
            sb.AppendLine("        projectdirtymask = 0;");
        }

        // 尾调基类 SGReset（仅 BehaviorInfo 子类）
        if (data.isBehaviorInfo)
        {
            sb.AppendLine("        base.SGReset();");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitClone(System.Text.StringBuilder sb, LifecycleClassData data)
    {
        var className = data.className;
        var methodName = data.isBehaviorInfo ? "SGClone" : "Clone";
        var returnType = data.isBehaviorInfo ? "BehaviorInfo" : "IGBL";
        var modifier = data.isBehaviorInfo ? "protected override" : "public";

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 深度克隆");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    {modifier} {returnType} {methodName}()");
        sb.AppendLine("    {");

        sb.AppendLine($"        var c = ObjectCache.Ensure<{className}>();");
        // Ready 先于字段拷贝：SGReady/OnReady 初始化容器，随后字段拷贝覆盖为目标值（仅 BehaviorInfo 子类）
        if (data.isBehaviorInfo)
        {
            sb.AppendLine("        c.Ready(actor);");
        }

        // ---- 值类型字段 ----
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ValueType) continue;
            sb.AppendLine($"        c.{field.name} = this.{field.name};");
        }

        // ---- 引用字段（浅拷贝） ----
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.Reference) continue;
            sb.AppendLine($"        c.{field.name} = this.{field.name};");
        }

        // ---- 容器值类型字段 ----
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerValue) continue;
            // GBL 容器用 Clone()，源 null 时显式分配空容器
            if (IsGBLContainer(field.typeName))
            {
                sb.AppendLine($"        c.{field.name} = null != this.{field.name}");
                sb.AppendLine($"            ? ({field.typeName})this.{field.name}.Clone()");
                sb.AppendLine($"            : ObjectCache.Ensure<{field.typeName}>();");
            }
            else
            {
                sb.AppendLine($"        c.{field.name} = null != this.{field.name} ? new {field.typeName}(this.{field.name}) : null;");
            }
        }

        // ---- 嵌套容器字段（内层值类型）：遍历外层，内层 new 拷贝 ----
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

        // ---- 嵌套容器字段（内层 IGBL）：遍历外层，内层 new + foreach Clone ----
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

        // ---- 容器 IGBL 字段 ----
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.ContainerIGBL) continue;

            // GBL 容器用 Clone() 方法，源 null 时显式分配空容器
            if (IsGBLContainer(field.typeName))
            {
                sb.AppendLine($"        c.{field.name} = null != this.{field.name}");
                sb.AppendLine($"            ? ({field.typeName})this.{field.name}.Clone()");
                sb.AppendLine($"            : ObjectCache.Ensure<{field.typeName}>();");
            }
            else
            {
                sb.AppendLine($"        if (null != this.{field.name})");
                sb.AppendLine("        {");
                sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
                sb.AppendLine($"            foreach (var item in this.{field.name})");
                sb.AppendLine($"                c.{field.name}.Add(({field.elementType})item.Clone());");
                sb.AppendLine("        }");
            }
        }

        // ---- IGBL 引用字段 ----
        foreach (var field in data.fields)
        {
            if (field.category != FieldCategory.IGBL) continue;
            sb.AppendLine($"        c.{field.name} = ({field.typeName})this.{field.name}?.Clone();");
        }

        // ---- Projector backing field 拷贝 ----
        foreach (var pf in data.projectorFields)
        {
            if (NeedsInit(pf.typeText))
                sb.AppendLine($"        c.{data.classNameLower}_{pf.name} = new {pf.typeText}();");
            else
                sb.AppendLine($"        c.{data.classNameLower}_{pf.name} = this.{data.classNameLower}_{pf.name};");
        }

        // 脏标记拷贝
        if (0 < data.projectorFields.Count)
        {
            sb.AppendLine("        c.projectdirtymask = this.projectdirtymask;");
        }

        // ---- 父类字段（用于继承链中非 BehaviorInfo 父类的字段拷贝） ----
        if (null != data.parentFields && 0 < data.parentFields.Count)
        {
            foreach (var field in data.parentFields)
            {
                switch (field.category)
                {
                    case FieldCategory.ValueType:
                    case FieldCategory.Reference:
                        sb.AppendLine($"        c.{field.name} = this.{field.name};");
                        break;

                    case FieldCategory.ContainerValue:
                        if (IsGBLContainer(field.typeName))
                        {
                            sb.AppendLine($"        c.{field.name} = null != this.{field.name}");
                            sb.AppendLine($"            ? ({field.typeName})this.{field.name}.Clone()");
                            sb.AppendLine($"            : ObjectCache.Ensure<{field.typeName}>();");
                        }
                        else
                        {
                            sb.AppendLine($"        c.{field.name} = null != this.{field.name} ? new {field.typeName}(this.{field.name}) : null;");
                        }
                        break;

                    case FieldCategory.ContainerIGBL:
                        if (IsGBLContainer(field.typeName))
                        {
                            sb.AppendLine($"        c.{field.name} = null != this.{field.name}");
                            sb.AppendLine($"            ? ({field.typeName})this.{field.name}.Clone()");
                            sb.AppendLine($"            : ObjectCache.Ensure<{field.typeName}>();");
                        }
                        else
                        {
                            sb.AppendLine($"        if (null != this.{field.name})");
                            sb.AppendLine("        {");
                            sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
                            sb.AppendLine($"            foreach (var item in this.{field.name})");
                            sb.AppendLine($"                c.{field.name}.Add(({field.elementType})item.Clone());");
                            sb.AppendLine("        }");
                        }
                        break;

                    case FieldCategory.ContainerNestedValue:
                        sb.AppendLine($"        if (null != this.{field.name})");
                        sb.AppendLine("        {");
                        sb.AppendLine($"            c.{field.name} = new {field.typeName}(this.{field.name}.Count);");
                        sb.AppendLine($"            foreach (var kv in this.{field.name})");
                        sb.AppendLine($"                c.{field.name}.Add(kv.Key, new {field.innerTypeName}(kv.Value));");
                        sb.AppendLine("        }");
                        break;

                    case FieldCategory.ContainerNestedIGBL:
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
                        break;

                    case FieldCategory.IGBL:
                        sb.AppendLine($"        c.{field.name} = ({field.typeName})this.{field.name}?.Clone();");
                        break;
                }
            }
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

        var nextAutoIndex = 0;
        foreach (var attrList in attributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var attrName = attr.Name.ToString();
                if (false == ("Projector" == attrName || "ProjectorAttribute" == attrName)) continue;

                var args = attr.ArgumentList?.Arguments;
                if (false == args.HasValue || args.Value.Count < 2) continue;

                var name = ExtractStringArg(args.Value[0]);
                if (null == name) continue;

                var typeText = ExtractTypeOfArg(args.Value[1]);
                if (null == typeText) continue;

                // index 可选（第三个非命名参数），不填时自动递增
                int? index = null;
                if (args.Value.Count >= 3 && null == args.Value[2].NameEquals)
                    index = ExtractIntArg(args.Value[2]);
                if (null == index) index = nextAutoIndex;
                nextAutoIndex = index.Value + 1;

                var summary = ExtractLeadingComment(attrList);

                fields.Add(new ProjectionFieldData(name, typeText, index.Value, summary));
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
        sb.AppendLine("using Goblin.Gameplay.Projection.Core;");
        sb.AppendLine();
        sb.AppendLine($"namespace {data.ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {data.className} : IProjectable");
        sb.AppendLine("{");

        var classNameLower = data.className.ToLowerInvariant();
        foreach (var field in data.fields)
        {
            var init = NeedsInit(field.typeText) ? $" = new {field.typeText}();" : "";
            sb.AppendLine($"    private {field.typeText} {classNameLower}_{field.name} {{ get; set; }}{init}");
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
            var ser = SerExpression(classNameLower, field.name, field.typeText);
            sb.AppendLine($"        if (0ul != (mask & (1ul << {field.index})))");
            sb.AppendLine($"            list.Add({ser});");
        }
        sb.AppendLine("        return list.ToArray();");
        sb.AppendLine("    }");

        sb.AppendLine();

        // SetProjectValues：从 object[] 恢复 backing field 值，不触发脏标记
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 从 object[] 设置 backing field 值，不触发脏标记");
        sb.AppendLine("    /// Phase 4 快照回滚时使用");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public void SetProjectValues(object[] values)");
        sb.AppendLine("    {");
        foreach (var field in data.fields)
        {
            var cast = CastExpression(field.typeText);
            sb.AppendLine($"        {classNameLower}_{field.name} = ({field.typeText}){cast}values[{field.index}];");
        }
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

    /// <summary>
    /// 按类型生成序列化表达式
    /// 所有类型直接装箱（Deserialize 端逐类型还原）
    /// </summary>
    private static string SerExpression(string backingPrefix, string fieldName, string typeText)
    {
        var backing = $"{backingPrefix}_{fieldName}";
        return typeText switch
        {
            "FP" => $"(object){backing}",
            "FPVector2" => $"(object){backing}",
            "FPVector3" => $"(object){backing}",
            "FPQuaternion" => $"(object){backing}",
            _ => backing
        };
    }

    /// <summary>
    /// 生成从 object 到目标类型的转换表达式
    /// 值类型需拆箱，FP 等 struct 直接强转
    /// </summary>
    private static string CastExpression(string typeText)
    {
        if (typeText is "int" or "long" or "float" or "double" or "bool" or "uint" or "ulong" or "short" or "ushort" or "byte" or "sbyte" or "char")
            return $"({typeText})";
        return string.Empty;
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
        if (expr is TypeOfExpressionSyntax typeOf)
        {
            // 简单类型，如 typeof(FP), typeof(FPVector3)
            if (typeOf.Type is IdentifierNameSyntax id)
                return id.Identifier.Text;
            // 限定类型，如 typeof(System.Action)
            if (typeOf.Type is QualifiedNameSyntax qn)
                return qn.ToString();
            // 泛型类型，如 typeof(GBLDict<uint, EffectInfo>)
            if (typeOf.Type is GenericNameSyntax gn)
                return gn.ToString();
            // 关键字类型，如 typeof(int), typeof(uint), typeof(bool)
            if (typeOf.Type is PredefinedTypeSyntax pts)
                return pts.Keyword.ValueText;
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

    /// <summary>
    /// 判断类型是否需要 new() 初始化（class 类型如 GBLList、GBLDict）
    /// </summary>
    private static bool NeedsInit(string typeText)
    {
        return typeText.StartsWith("GBLList<") || typeText.StartsWith("GBLDict<");
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

    // ============================================================
    // 管线 3：Component ApplyTo 生成
    // ============================================================

    /// <summary>
    /// ApplyTo 生成数据
    /// </summary>
    private sealed record ApplyToData(
        string ns,
        string className,
        List<ApplyToFieldData> fields
    );

    /// <summary>
    /// ApplyTo 字段数据
    /// </summary>
    private sealed record ApplyToFieldData(
        string name,
        string typeText,
        int index,
        string? targetTypeText = null
    );

    /// <summary>
    /// 过滤：partial class 带 [ProjectorTarget] 注解
    /// </summary>
    private static bool IsPartialClassWithProjectorTarget(SyntaxNode node)
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
                if ("ProjectorTarget" == name || "ProjectorTargetAttribute" == name)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 提取 ApplyTo 生成数据：读取 [ProjectorTarget] 中指定的 BehaviorInfo 的 [Projector] 字段
    /// </summary>
    private static ApplyToData? ExtractApplyToData(GeneratorSyntaxContext ctx)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;
        var model = ctx.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDecl);
        if (null == symbol) return null;

        var ns = symbol.ContainingNamespace.ToDisplayString();
        var className = symbol.Name;

        // 查找 [ProjectorTarget] 属性
        ITypeSymbol? targetInfoType = null;
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name is "ProjectorTargetAttribute" or "ProjectorTarget")
            {
                if (attr.ConstructorArguments.Length > 0)
                {
                    targetInfoType = attr.ConstructorArguments[0].Value as ITypeSymbol;
                    break;
                }
            }
        }
        if (null == targetInfoType) return null;

        // 收集目标 BehaviorInfo 的 [Projector] 字段
        var fields = new List<ApplyToFieldData>();
        var nextAutoIndex = 0;
        foreach (var attr in targetInfoType.GetAttributes())
        {
            if (attr.AttributeClass?.Name is not "ProjectorAttribute" and not "Projector") continue;

            var cargs = attr.ConstructorArguments;
            if (cargs.Length < 2) continue;

            var name = cargs[0].Value as string;
            if (null == name) continue;

            var typeObj = cargs[1].Value as ITypeSymbol;
            var typeText = typeObj?.ToDisplayString();
            if (null == typeText) continue;

            var index = cargs.Length >= 3 && cargs[2].Value is int intVal && -1 != intVal
                ? intVal
                : nextAutoIndex;
            nextAutoIndex = index + 1;

            // 查找 Component 上同名属性的类型，检测 GBL→原生容器转换需求
            string? targetTypeText = null;
            foreach (var member in symbol.GetMembers(name))
            {
                if (member is IPropertySymbol prop)
                {
                    var propTypeText = prop.Type.ToDisplayString();
                    if (propTypeText != typeText)
                    {
                        targetTypeText = propTypeText;
                    }
                    break;
                }
            }

            fields.Add(new ApplyToFieldData(name, typeText, index, targetTypeText));
        }

        if (0 == fields.Count) return null;

        return new ApplyToData(ns, className, fields);
    }

    /// <summary>
    /// 生成 ApplyTo 静态类 + Component 静态构造函数
    /// </summary>
    private static void EmitApplyToCode(SourceProductionContext context, ApplyToData data)
    {
        var ns = data.ns;
        var cn = data.className;
        var sb = new System.Text.StringBuilder();

        // ---- 文件 1：{Name}Apply 静态类 ----
        sb.Clear();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"internal static class {cn}Apply");
        sb.AppendLine("{");
        sb.AppendLine("    internal static void ApplyTo(object comp, ulong fieldmask, object[] values)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var c = ({cn})comp;");
        sb.AppendLine("        var vi = 0;");

        foreach (var field in data.fields)
        {
            sb.AppendLine($"        // Bit{field.index}: {field.name}");
            if (null != field.targetTypeText)
            {
                // GBL 容器 → 原生容器转换
                sb.AppendLine($"        if (0 != (fieldmask & (1UL << {field.index}))) c.{field.name} = new {field.targetTypeText}(({field.typeText})values[vi++]);");
            }
            else
            {
                sb.AppendLine($"        if (0 != (fieldmask & (1UL << {field.index}))) c.{field.name} = ({field.typeText})values[vi++];");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource($"{ns}.{cn}Apply.g.cs", sb.ToString());

        // ---- 文件 2：Component 接口实现（自动注册 ApplyTo） ----
        sb.Clear();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");
        sb.AppendLine();
        sb.AppendLine($"partial class {cn} : Goblin.Gameplay.Render.Components.IComponentApply<{cn}>");
        sb.AppendLine("{");
        sb.AppendLine($"    static Action<object, ulong, object[]> Goblin.Gameplay.Render.Components.IComponentApply<{cn}>.ApplyTo");
        sb.AppendLine($"        => {cn}Apply.ApplyTo;");
        sb.AppendLine("}");

        context.AddSource($"{ns}.{cn}.applyreg.g.cs", sb.ToString());
    }
}

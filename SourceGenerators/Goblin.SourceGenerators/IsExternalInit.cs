// netstandard2.0 缺少 IsExternalInit，C# 9+ record 类型需要此类型
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

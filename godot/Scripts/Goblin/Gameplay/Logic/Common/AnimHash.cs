using System.Text;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 动画名称哈希（FNV-1a，跨平台确定性）
/// </summary>
public static class AnimHash
{
    private const uint FNV_PRIME = 16777619;
    private const uint FNV_OFFSET = 2166136261;

    /// <summary>
    /// 计算动画名称的确定性哈希
    /// </summary>
    /// <param name="name">动画名称</param>
    /// <returns>uint 哈希值（null 或空字符串返回 0）</returns>
    public static uint Hash(string name)
    {
        if (string.IsNullOrEmpty(name)) return 0;

        var bytes = Encoding.UTF8.GetBytes(name);
        var hash = FNV_OFFSET;
        foreach (var b in bytes)
        {
            hash ^= b;
            hash *= FNV_PRIME;
        }

        return hash;
    }
}

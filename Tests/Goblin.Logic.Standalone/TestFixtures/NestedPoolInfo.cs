using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 嵌套池化对象（IGBL 非 BehaviorInfo）
/// </summary>
public class PooledItem : IGBL
{
    public int x { get; set; }
    public int y { get; set; }

    public void Reset()
    {
        x = 0;
        y = 0;
    }

    public IGBL Clone()
    {
        var c = ObjectCache.Ensure<PooledItem>();
        c.x = x;
        c.y = y;
        return c;
    }
}

/// <summary>
/// 模式 3：嵌套池化对象
/// SG 生成 IGBL 元素 → foreach item.Reset() + ObjectCache.Set + items.Clear()
/// override Clone() → foreach Add((PooledItem)src[i].Clone())
/// </summary>
public partial class NestedPoolInfo : BehaviorInfo
{
    public List<PooledItem> items { get; set; }
}

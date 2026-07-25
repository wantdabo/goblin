using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;

namespace Goblin.Gameplay.Logic.Prefabs;

/// <summary>
/// 魔法体预制信息
/// </summary>
public struct MagicPrefabInfo : IPrefabInfo
{
    /// <summary>
    /// 施法者 ActorID
    /// </summary>
    public ulong owner { get; set; }
    /// <summary>
    /// 空间信息（必填）
    /// </summary>
    public SpatialData spatial { get; set; }
    /// <summary>
    /// 管线列表
    /// </summary>
    public GBLList<uint> pipelines { get; set; }
}

/// <summary>
/// 魔法体预制创建器
/// </summary>
public class MagicPrefab : Prefab<MagicPrefabInfo>
{
    public override byte type => ACTOR_DEFINE.MAGIC;

    protected override void OnProcessing(ulong actor, MagicPrefabInfo info)
    {
        var magic = stage.AddBehaviorInfo<MagicInfo>(actor);
        magic.owner = info.owner;

        var spatial = stage.AddBehaviorInfo<SpatialInfo>(actor);
        spatial.position = info.spatial.position;
        spatial.euler = info.spatial.euler;
        spatial.scale = info.spatial.scale;

        var pipelines = ObjectCache.Ensure<GBLList<uint>>();
        pipelines.AddRange(info.pipelines);
        magic.flow = stage.flow.GenPipeline(actor, pipelines);
    }
}

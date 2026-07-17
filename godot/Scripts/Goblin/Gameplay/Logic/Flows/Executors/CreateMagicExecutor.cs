using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 生成魔法体执行器
/// </summary>
public class CreateMagicExecutor : Executor<CreateMagicData>
{
    protected override void OnExecute((uint pipelineid, uint index) identity, CreateMagicData data, FlowInfo flowinfo, ulong target)
    {
        base.OnExecute(identity, data, flowinfo, target);
        if (false == stage.SeekBehaviorInfo(target, out SpatialInfo spatial)) return;

        var spatialdata = new SpatialData();
        switch (data.origin)
        {
            case FLOW_MAGIC_DEFINE.BORN_ORIGIN_OWNER:
                spatialdata.position = spatial.position;
                break;
        }
        spatialdata.position += data.offset.ToFPVector3();

        switch (data.euler)
        {
            case FLOW_MAGIC_DEFINE.BORN_EULER_OWNER:
                spatialdata.euler = spatial.euler;
                break;
        }
        spatialdata.euler = new FPVector3(spatial.euler.x, spatial.euler.y + (data.angle * stage.cfg.int2fp), spatial.euler.z);
        spatialdata.scale = data.scale * stage.cfg.int2fp;

        // 子 Magic 的 owner 继承自父 Magic 的 owner，否则用 target 本身
        var owner = target;
        if (stage.SeekBehaviorInfo(target, out MagicInfo parentmagic)) owner = parentmagic.owner;

        stage.Spawn(new MagicPrefabInfo
        {
            owner = owner,
            spatial = spatialdata,
            pipelines = data.pipelines,
        });
    }
}

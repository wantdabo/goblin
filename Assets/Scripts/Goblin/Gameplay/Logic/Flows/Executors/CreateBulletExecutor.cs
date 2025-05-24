using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Collisions;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 创建子弹执行器
    /// </summary>
    public class CreateBulletExecutor : Executor<CreateBulletData>
    {
        protected override void OnEnter(CreateBulletData data, FlowInfo flowinfo)
        {
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            var spatialdata = new SpatialData();
            switch (data.origin)
            {
                case FLOW_BULLET_DEFINE.BORN_ORIGIN_OWNER:
                    spatialdata.position = spatial.position;
                    break;
            }
            switch (data.euler)
            {
                case FLOW_BULLET_DEFINE.BORN_EULER_OWNER:
                    spatialdata.euler = spatial.euler;
                    break;
            }
            spatialdata.scale = data.scale * stage.cfg.int2fp;

            stage.Spawn(new BulletPrefabInfo
            {
                owner = flowinfo.owner,
                strength = data.strength * stage.cfg.int2fp,
                speed = data.speed * stage.cfg.int2fp,
                spatial = spatialdata,
                pipelines = data.pipelines,
            });
        }
    }
}
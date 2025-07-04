using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Extensions;
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
        protected override void OnExecute((uint pipelineid, uint index) identity, CreateBulletData data, FlowInfo flowinfo)
        {
            base.OnExecute(identity, data, flowinfo);
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            
            var spatialdata = new SpatialData();
            switch (data.origin)
            {
                case FLOW_BULLET_DEFINE.BORN_ORIGIN_OWNER:
                    spatialdata.position = spatial.position;
                    break;
            }
            spatialdata.position += data.offset.ToFPVector3();

            switch (data.euler)
            {
                case FLOW_BULLET_DEFINE.BORN_EULER_OWNER:
                    spatialdata.euler = spatial.euler;
                    break;
            }
            spatialdata.euler = new FPVector3(spatial.euler.x, spatial.euler.y + (data.angle * stage.cfg.int2fp), spatial.euler.z);
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
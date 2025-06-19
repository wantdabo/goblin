using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 碰撞执行器
    /// </summary>
    public class CollisionExecutor : Executor<CollisionData>
    {
        protected override void OnExecute((uint pipelineid, uint index) identity, CollisionData data, FlowInfo flowinfo)
        {
            base.OnExecute(identity, data, flowinfo);

            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            
            var rotation = FPQuaternion.Euler(spatial.euler);
            var position = spatial.position + rotation * data.offset.ToFPVector3();

            // 碰撞检测
            HitResult result = default;
            switch (data.overlaptype)
            {
                case COLLISION_DEFINE.COLLISION_RAY:
                    result = stage.detection.Raycast(position, rotation * data.raydire.ToFPVector3(), data.raydis * stage.cfg.int2fp);
                    break;
                case COLLISION_DEFINE.COLLISION_LINE:
                    result = stage.detection.Linecast(position, position + rotation * data.lineep.ToFPVector3());
                    break;
                case COLLISION_DEFINE.COLLISION_BOX:
                    result = stage.detection.OverlapBox(position, FPQuaternion.identity, data.boxsize.ToFPVector3());
                    break;
                case COLLISION_DEFINE.COLLISION_SPHERE:
                    result = stage.detection.OverlapSphere(position, data.sphereradius);
                    break;
            }
            if (false == result.hit) return;
            
            // 处理碰撞结果
            switch (data.type)
            {
                case COLLISION_DEFINE.COLLISION_TYPE_HURT:
                    OnCollisionHurt(result, identity, data, flowinfo);
                    break;
                case COLLISION_DEFINE.COLLISION_TYPE_SENSOR:
                    OnCollisionSensor(result, identity, data, flowinfo);
                    break;
            }
        }

        /// <summary>
        /// 处理攻击盒
        /// </summary>
        /// <param name="result">碰撞结果</param>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        private void OnCollisionHurt(HitResult result, (uint pipelineid, uint index) identity, CollisionData data, FlowInfo flowinfo)
        {
            if (false == result.hit) return;
            
            if (false == stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo collisionhurt)) collisionhurt = stage.AddBehaviorInfo<FlowCollisionHurtInfo>(flowinfo.actor);
            if (false == collisionhurt.records.TryGetValue(identity, out var record)) collisionhurt.records.Add(identity, record = ObjectCache.Ensure<Dictionary<ulong, uint>>());
            foreach (var collider in result.colliders)
            {
                var has = record.TryGetValue(collider.actor, out var count);
                if (count >= data.count) continue;
                count++;
                
                if (has) record.Remove(collider.actor);
                record.Add(collider.actor, count);
                
                collisionhurt.targets.Enqueue((collider.actor, identity));
            }
        }
        
        /// <summary>
        /// 处理嗅探器
        /// </summary>
        /// <param name="result">碰撞结果</param>
        /// <param name="identity">(管线 ID, 指令索引)</param>
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        private void OnCollisionSensor(HitResult result, (uint pipelineid, uint index) identity, CollisionData data, FlowInfo flowinfo)
        {
        }
    }
}
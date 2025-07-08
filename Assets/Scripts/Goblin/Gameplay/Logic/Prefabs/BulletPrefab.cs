using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Prefabs
{
    /// <summary>
    /// 子弹预制信息
    /// </summary>
    public struct BulletPrefabInfo : IPrefabInfo
    {
        /// <summary>
        /// 子弹的拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// 子弹的伤害强度
        /// </summary>
        public FP strength { get; set; }
        /// <summary>
        /// 子弹的速度
        /// </summary>
        public FP speed { get; set; }
        /// <summary>
        /// 空间信息
        /// </summary>
        public SpatialData spatial { get; set; }
        /// <summary>
        /// 管线列表
        /// </summary>
        public List<uint> pipelines { get; set; }
    }

    /// <summary>
    /// 子弹预制创建器
    /// </summary>
    public class BulletPrefab : Prefab<BulletPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.BULLET;

        protected override void OnProcessing(ulong actor, BulletPrefabInfo info)
        {
            var bullet = stage.AddBehaviorInfo<BulletInfo>(actor);
            bullet.owner = info.owner;
            bullet.strength = info.strength;
            bullet.speed = info.speed;
            bullet.damage = stage.attrc.ChargeDamage(bullet.owner, bullet.strength);
            
            var spatial = stage.AddBehaviorInfo<SpatialInfo>(actor);
            spatial.position = info.spatial.position;
            spatial.euler = info.spatial.euler;
            spatial.scale = info.spatial.scale;
            
            // TODO 临时加模型, 记得删除
            var facade = stage.AddBehavior<Facade>(actor);
            facade.SetModel(200001);
            
            var pipelines = ObjectCache.Ensure<List<uint>>();
            pipelines.AddRange(info.pipelines);
            bullet.flow = stage.flow.GenPipeline(actor, pipelines);
        }
    }
}
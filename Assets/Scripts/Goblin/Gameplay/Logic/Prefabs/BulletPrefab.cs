using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Prefabs
{
    /// <summary>
    /// 子弹预制体信息
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
        /// 子弹的移动速度
        /// </summary>
        public FP movespeed { get; set; }
        /// <summary>
        /// 管线列表
        /// </summary>
        public List<uint> pipelines { get; set; }
        /// <summary>
        /// 空间信息
        /// </summary>
        public SpatialData spatial { get; set; }
    }

    /// <summary>
    /// 子弹预制体
    /// </summary>
    public class BulletPrefab : Prefab<BulletPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.BULLET;

        protected override void OnProcessing(Actor actor, BulletPrefabInfo info)
        {
            actor.AddBehavior<Bullet>();

            actor.SeekBehaviorInfo(out BulletInfo bullet);
            bullet.owner = info.owner;
            bullet.flow = stage.flow.GenPipeline(actor.id, info.pipelines).id;
            bullet.strength = info.strength;
            bullet.moveseepd = info.movespeed;
            bullet.damage = stage.calc.ChargeDamage(bullet.owner, bullet.strength);
            
            var spatial = actor.AddBehaviorInfo<SpatialInfo>();
            spatial.position = info.spatial.position;
            spatial.euler = info.spatial.euler;
            spatial.scale = info.spatial.scale;
        }
    }
}
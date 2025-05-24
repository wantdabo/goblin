using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Collisions;
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
    /// 英雄预设信息结构体
    /// </summary>
    public struct HeroPrefabInfo : IPrefabInfo
    {
        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }
        /// <summary>
        /// 空间信息
        /// </summary>
        public SpatialData spatial { get; set; }
    }

    /// <summary>
    /// 英雄预制创建器
    /// </summary>
    public class HeroPrefab : Prefab<HeroPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.HERO;

        protected override void OnProcessing(Actor actor, HeroPrefabInfo info)
        {
            var herocfg = stage.cfg.location.HeroInfos.Get(info.hero);
            var attrcfg = stage.cfg.location.AttributeInfos.Get(herocfg.Attribute);

            actor.AddBehavior<StateMachine>();
            actor.AddBehavior<Movement>();
            
            var launcher = actor.AddBehavior<SkillLauncher>();
            // 设置技能释放器的技能
            if (null != herocfg.Skills)
            {
                foreach (var skill in herocfg.Skills)
                {
                    var data = stage.cfg.location.SkillInfos.Get(skill);
                    if (null == data) continue;
                    
                    var strength = data.Strength * stage.cfg.int2fp;
                    var cooldown = data.Cooldown * stage.cfg.int2fp;
                    var pipelines = ObjectCache.Ensure<List<uint>>();
                    foreach (var pipeline in data.Pipelines) pipelines.Add((uint)pipeline);
                    
                    launcher.Load((uint)skill, strength, cooldown, pipelines);
                }
            }

            var facade = actor.AddBehavior<Facade>();
            facade.SetModel(herocfg.Model);

            actor.AddBehaviorInfo<TickerInfo>();
            var attribute = actor.AddBehaviorInfo<AttributeInfo>();
            attribute.hp = (uint)attrcfg.HP;
            attribute.maxhp = (uint)attrcfg.HP;
            attribute.movespeed = (uint)attrcfg.MoveSpeed;
            attribute.attack = (uint)attrcfg.Attack;
            
            var spatial = actor.AddBehaviorInfo<SpatialInfo>();
            spatial.position = info.spatial.position;
            spatial.euler = info.spatial.euler;
            spatial.scale = info.spatial.scale;

            var collider = actor.AddBehaviorInfo<ColliderInfo>();
            stage.detection.SetColliderInfo(collider, herocfg.Collider);
        }
    }
}
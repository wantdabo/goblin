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
    /// 英雄预制信息
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

        public override bool born => true;

        protected override void OnProcessing(ulong actor, HeroPrefabInfo info)
        {
            if (false == stage.cfg.location.HeroInfos.TryGetValue(info.hero, out var herocfg)) return;
            if (false == stage.cfg.location.AttributeInfos.TryGetValue(herocfg.Attribute, out var attrcfg)) return;

            stage.AddBehavior<StateMachine>(actor);
            stage.AddBehavior<Movement>(actor);
            
            var launcher = stage.AddBehavior<SkillLauncher>(actor);
            // 设置技能释放器的技能
            if (null != herocfg.Skills)
            {
                foreach (var skill in herocfg.Skills)
                {
                    if (false == stage.cfg.location.SkillInfos.TryGetValue(skill, out var skillcfg)) return;
                    if (null == skillcfg) continue;
                    
                    var strength = skillcfg.Strength * stage.cfg.int2fp;
                    var cooldown = skillcfg.Cooldown * stage.cfg.int2fp;
                    var pipelines = ObjectCache.Ensure<List<uint>>();
                    foreach (var pipeline in skillcfg.Pipelines) pipelines.Add((uint)pipeline);
                    launcher.Load((uint)skill, strength, cooldown, pipelines);
                }
            }

            var facade = stage.AddBehavior<Facade>(actor);
            facade.SetModel(herocfg.Model);

            stage.AddBehaviorInfo<TickerInfo>(actor);
            var attribute = stage.AddBehaviorInfo<AttributeInfo>(actor);
            stage.attrc.SetAttributeValue(attribute, ATTRIBUTE_DEFINE.HP, attrcfg.HP);
            stage.attrc.SetAttributeValue(attribute, ATTRIBUTE_DEFINE.MAXHP, attrcfg.MaxHP);
            stage.attrc.SetAttributeValue(attribute, ATTRIBUTE_DEFINE.MOVESPEED, attrcfg.MoveSpeed);
            stage.attrc.SetAttributeValue(attribute, ATTRIBUTE_DEFINE.ATTACK, attrcfg.Attack);
            
            var spatial = stage.AddBehaviorInfo<SpatialInfo>(actor);
            spatial.position = info.spatial.position;
            spatial.euler = info.spatial.euler;
            spatial.scale = info.spatial.scale;

            var collider = stage.AddBehaviorInfo<ColliderInfo>(actor);
            stage.detection.SetColliderInfo(collider, herocfg.Collider);
        }
    }
}
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;

namespace Goblin.Gameplay.Logic.Prefabs;

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

    protected override void OnProcessing(ulong actor, HeroPrefabInfo info)
    {
        if (false == stage.cfg.location.HeroInfos.TryGetValue(info.hero, out var herocfg)) return;
        if (false == stage.cfg.location.AttributeInfos.TryGetValue(herocfg.Attribute, out var attrbfg)) return;

        stage.AddBehavior<StateMachine>(actor);
        stage.AddBehavior<Movement>(actor);
        stage.AddBehavior<HUD>(actor);
            
        stage.AddBehavior<SkillLauncher>(actor);

        var facade = stage.AddBehavior<Facade>(actor);
        facade.SetModel(herocfg.Model);

        stage.AddBehaviorInfo<TickerInfo>(actor);
        stage.attrb.Attach(actor);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.HP, attrbfg.HP);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.MAXHP, attrbfg.MaxHP);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED, attrbfg.MoveSpeed);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK, attrbfg.Attack);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.ARMOR, attrbfg.Armor);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.MAGIC_RESIST, attrbfg.MagicResist);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.CRIT_RATE, attrbfg.CritRate);
        stage.attrb.SetAttributeValue(actor, ATTRIBUTE_DEFINE.DODGE_RATE, attrbfg.DodgeRate);
            
        var spatial = stage.AddBehaviorInfo<SpatialInfo>(actor);
        spatial.position = info.spatial.position;
        spatial.euler = info.spatial.euler;
        spatial.scale = info.spatial.scale;

        var collider = stage.AddBehaviorInfo<ColliderInfo>(actor);
        stage.detection.SetColliderInfo(collider, herocfg.Collider);

        Career(actor, herocfg.BornPipelines, herocfg.DeathPipelines);
    }
}
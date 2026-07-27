using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Prefabs.Datas;

namespace Goblin.Gameplay.Logic.Prefabs;

/// <summary>
/// 敌人预制信息
/// </summary>
public struct EnemyPrefabInfo : IPrefabInfo
{
    /// <summary>
    /// 敌人 ID（对应 EnemyInfo.Id）
    /// </summary>
    public int enemy { get; set; }
    /// <summary>
    /// 空间信息
    /// </summary>
    public SpatialData spatial { get; set; }
}

/// <summary>
/// 敌人预制创建器
/// </summary>
public class EnemyPrefab : Prefab<EnemyPrefabInfo>
{
    public override byte type => ACTOR_DEFINE.ENEMY;

    protected override void OnProcessing(ulong actor, EnemyPrefabInfo info)
    {
        if (false == stage.cfg.location.EnemyInfos.TryGetValue(info.enemy, out var enemycfg)) return;
        if (false == stage.cfg.location.AttributeInfos.TryGetValue(enemycfg.Attribute, out var attrbfg)) return;

        stage.AddBehavior<StateMachine>(actor);
        stage.AddBehavior<Movement>(actor);
        stage.AddBehavior<HUD>(actor);

        stage.AddBehavior<SkillLauncher>(actor);

        var facade = stage.AddBehavior<Facade>(actor);
        facade.SetModel(enemycfg.Model);

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
        stage.detection.SetColliderInfo(collider, enemycfg.Collider);

        Career(actor, enemycfg.BornPipelines, enemycfg.DeathPipelines);
    }
}

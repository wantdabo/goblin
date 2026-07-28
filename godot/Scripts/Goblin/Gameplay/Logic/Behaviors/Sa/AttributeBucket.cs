using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 伤害的数据结构
/// </summary>
public struct DamageInfo
{
    public bool crit { get; set; }
    public bool magic { get; set; }
    public int value { get; set; }
}

/// <summary>
/// 属性桶（Sa 级，统一管理所有 Actor 的属性数据）
/// </summary>
public class AttributeBucket : Behavior<AttributeBucketInfo>
{
    static AttributeBucket()
    {
        Eventor.Listen<ActorRmvEvent>(OnActorRmv);
    }

    /// <summary>
    /// 将 Actor 接入属性桶
    /// </summary>
    public void Attach(ulong actor)
    {
        if (info.attributes.ContainsKey(actor)) return;
        info.attributes.Add(actor, ObjectCache.Ensure<GBLDict<ushort, int>>());
    }

    private (ushort mainkey, ushort scalekey) ConvKey(ushort key)
    {
        return ((ushort)(key * 2 + 1), (ushort)(key * 2 + 2));
    }

    public int GetAttributeValue(ulong actor, ushort key)
    {
        if (false == info.attributes.TryGetValue(actor, out var attributes)) return 0;
        var k = ConvKey(key);
        var value = attributes.GetValueOrDefault(k.mainkey, 0);
        var scale = attributes.GetValueOrDefault(k.scalekey, 1000);

        return Math.Clamp((value * (scale * stage.cfg.int2fp)).AsInt(), 0, int.MaxValue);
    }

    public int GetAttributeScaleValue(ulong actor, ushort key)
    {
        if (false == info.attributes.TryGetValue(actor, out var attributes)) return 1000;
        var k = ConvKey(key);

        return attributes.GetValueOrDefault(k.scalekey, 1000);
    }

    public void SetAttributeValue(ulong actor, ushort key, int value)
    {
        if (false == info.attributes.TryGetValue(actor, out var attributes)) return;
        var k = ConvKey(key);
        attributes.Remove(k.mainkey);
        attributes.Add(k.mainkey, value);
    }

    public void SetAttributeScaleValue(ulong actor, ushort key, int value)
    {
        if (ATTRIBUTE_DEFINE.HP == key) throw new Exception("HP 属性的千分比值不允许被修改");
        if (false == info.attributes.TryGetValue(actor, out var attributes)) return;
        var k = ConvKey(key);
        attributes.Remove(k.scalekey);
        attributes.Add(k.scalekey, value);
    }

    public (int before, int after) ChangeAttributeValue(ulong actor, ushort key, int value, bool clamp = false, int min = 0, int max = 0)
    {
        var before = GetAttributeValue(actor, key);
        var changevalue = before + value;
        if (clamp) changevalue = Math.Clamp(changevalue, min, max);
        SetAttributeValue(actor, key, changevalue);
        var after = GetAttributeValue(actor, key);

        return (before, after);
    }

    public (int before, int after) ChangeAttributeScaleValue(ulong actor, ushort key, int value, bool clamp = false, int min = 0, int max = 0)
    {
        var before = GetAttributeScaleValue(actor, key);
        var changevalue = before + value;
        if (clamp) changevalue = Math.Clamp(changevalue, min, max);
        SetAttributeScaleValue(actor, key, changevalue);
        var after = GetAttributeScaleValue(actor, key);

        return (before, after);
    }

    public DamageInfo ChargeDamage(ulong actor, FP strength)
    {
        if (false == info.attributes.ContainsKey(actor)) return default;

        // 暴击判定（千分比，如 200 = 20%）
        var critrate = GetAttributeValue(actor, ATTRIBUTE_DEFINE.CRIT_RATE);
        var crit = critrate > 0 && stage.random.Range(0, 1000) < critrate;

        return new DamageInfo
        {
            crit = crit,
            value = FP.ToInt(strength * GetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK))
        };
    }

    public DamageInfo DischargeDamage(ulong actor, DamageInfo damage)
    {
        if (false == info.attributes.ContainsKey(actor)) return damage;

        // 闪避判定（千分比，如 200 = 20%）
        var dodgerate = GetAttributeValue(actor, ATTRIBUTE_DEFINE.DODGE_RATE);
        if (dodgerate > 0 && stage.random.Range(0, 1000) < dodgerate)
        {
            damage.value = 0;
            return damage;
        }

        // 减伤：物理用护甲，魔法用魔抗（固定值减伤）
        var resist = damage.magic
            ? GetAttributeValue(actor, ATTRIBUTE_DEFINE.MAGIC_RESIST)
            : GetAttributeValue(actor, ATTRIBUTE_DEFINE.ARMOR);
        damage.value = Math.Max(0, damage.value - resist);

        return damage;
    }

    public void ToDamage(ulong from, ulong to, DamageInfo damage)
    {
        if (stage.SeekBehaviorInfo(to, out StateMachineInfo statemachine) && STATE_DEFINE.DEATH == statemachine.current) return;
        if (false == info.attributes.ContainsKey(to)) return;

        var disdamage = DischargeDamage(to, damage);
        var result = ChangeAttributeValue(to, ATTRIBUTE_DEFINE.HP, -disdamage.value, true, 0, GetAttributeValue(to, ATTRIBUTE_DEFINE.MAXHP));

        if (result.after > 0) return;
        stage.silentmercy.Kill(from, to);
    }

    protected override void OnEndTick()
    {
        base.OnEndTick();
        if (0 == info.pendings.Count) return;

        // 检查 pending 中的 actor 是否还被 Magic 引用，没有则真正回收
        var done = ObjectCache.Ensure<GBLList<ulong>>();
        if (stage.SeekBehaviorInfos(out List<MagicInfo> magics))
        {
            foreach (var pending in info.pendings)
            {
                var inuse = false;
                foreach (var magic in magics)
                {
                    if (magic.owner != pending) continue;
                    inuse = true;
                    break;
                }
                if (inuse) continue;
                done.Add(pending);
            }
        }
        else
        {
            done.AddRange(info.pendings);
        }

        foreach (var actor in done)
        {
            info.pendings.Remove(actor);
            if (false == info.attributes.TryGetValue(actor, out var attributes)) continue;
            attributes.Clear();
            ObjectCache.Set(attributes);
            info.attributes.Remove(actor);
        }

        done.Dispose();
    }

    private static void OnActorRmv(Stage stage, ActorRmvEvent e)
    {
        if (false == stage.attrb.info.attributes.ContainsKey(e.actor)) return;
        if (stage.attrb.info.pendings.Contains(e.actor)) return;
        stage.attrb.info.pendings.Add(e.actor);
    }
}

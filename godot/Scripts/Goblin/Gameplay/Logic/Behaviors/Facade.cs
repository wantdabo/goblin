using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 外观行为（Sa 级）
/// 管理所有 Actor 的模型、动画、特效
/// </summary>
public class Facade : Behavior
{
    /// <summary>
    /// 设置模型
    /// </summary>
    public void SetModel(ulong actor, int model)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        info.model = model;
    }

    /// <summary>
    /// 设置动画状态
    /// </summary>
    public void SetAnimation(ulong actor, byte state)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        info.animstate = state;
        info.animhash = 0;
        info.animelapsed = 0;
        var priority = STATE_DEFINE.DEATH == state || STATE_DEFINE.BORN == state
            ? ANIM_DEFINE.SLOT_PRIORITY_LIFESTATE
            : ANIM_DEFINE.SLOT_PRIORITY_LOCOMOTION;
        AddOrUpdateSlot(info, ANIM_DEFINE.SLOT_TYPE_STATE, priority, state: state);
        RmvSlotsByType(actor, ANIM_DEFINE.SLOT_TYPE_OVERRIDE);
    }

    /// <summary>
    /// 设置动画名称
    /// </summary>
    public void SetAnimation(ulong actor, string animname, byte ticktype = ANIM_DEFINE.TICK_AUTOMATIC, byte layer = ANIM_DEFINE.LAYER_FULLBODY)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        info.animticktype = ticktype;
        info.animhash = AnimHash.Hash(animname);
        info.animelapsed = 0;
        if (null != animname)
            AddOrUpdateSlot(info, ANIM_DEFINE.SLOT_TYPE_NAMED, ANIM_DEFINE.SLOT_PRIORITY_ACTION, namehash: info.animhash, layer: layer);
        else
            RmvSlot(actor, ANIM_DEFINE.GenKey(ANIM_DEFINE.SLOT_TYPE_NAMED, layer));
    }

    /// <summary>
    /// 添加或更新槽位
    /// </summary>
    public void AddOrUpdateSlot(ulong actor, byte slottype, int priority, byte state = 0, uint namehash = 0, byte layer = ANIM_DEFINE.LAYER_FULLBODY, FP duration = default)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        AddOrUpdateSlot(info, slottype, priority, state, namehash, layer, duration);
    }

    private void AddOrUpdateSlot(FacadeInfo info, byte slottype, int priority, byte state = 0, uint namehash = 0, byte layer = ANIM_DEFINE.LAYER_FULLBODY, FP duration = default)
    {
        var key = ANIM_DEFINE.GenKey(slottype, layer);
        var slot = GetSlot(info, key);
        if (null == slot)
        {
            slot = ObjectCache.Ensure<AnimationSlot>();
            slot.key = key;
            info.animslots.Add(slot);
        }
        slot.priority = priority;
        slot.active = true;
        slot.animstate = state;
        slot.animhash = namehash;
        slot.layer = layer;
        slot.elapsed = FP.Zero;
        if (FP.Zero < duration) { slot.istransient = true; slot.duration = duration; }
        else { slot.istransient = false; slot.duration = FP.Zero; }
        EnsureSort(info);
    }

    /// <summary>
    /// 移除槽位
    /// </summary>
    public void RmvSlot(ulong actor, ushort key)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        var slot = GetSlot(info, key);
        if (null != slot) ReleaseSlot(info, slot);
    }

    /// <summary>
    /// 按槽位类型移除所有匹配槽位
    /// </summary>
    public void RmvSlotsByType(ulong actor, byte slottype)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return;
        for (int i = info.animslots.Count - 1; i >= 0; i--)
        {
            if (ANIM_DEFINE.GetSlotType(info.animslots[i].key) != slottype) continue;
            ReleaseSlot(info, info.animslots[i]);
        }
    }

    private void ReleaseSlot(FacadeInfo info, AnimationSlot slot)
    {
        info.animslots.Remove(slot);
        slot.active = false;
        slot.animstate = 0;
        slot.animhash = 0;
        slot.layer = 0;
        slot.istransient = false;
        slot.duration = FP.Zero;
        slot.elapsed = FP.Zero;
        ObjectCache.Set(slot);
    }

    private void EnsureSort(FacadeInfo info) => info.animslots.Sort((a, b) => b.priority.CompareTo(a.priority));

    private AnimationSlot GetSlot(FacadeInfo info, ushort key)
    {
        foreach (var slot in info.animslots)
            if (slot.key == key) return slot;
        return null;
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    public uint CreateEffect(ulong actor, EffectInfo effect)
    {
        if (false == stage.SeekBehaviorInfo(actor, out FacadeInfo info)) return 0;
        var increment = info.effectincrement++;
        effect.id = increment;
        effect.elapsed = 0;
        info.effectdict.Add(effect.id, effect);

        return increment;
    }

    protected override void OnTick(FP tick)
    {
        if (false == stage.SeekBehaviorInfos(out List<FacadeInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;

            if (info.animticktype == ANIM_DEFINE.TICK_AUTOMATIC) info.animelapsed += tick;

            // 逐槽位递进 elapsed
            for (int i = info.animslots.Count - 1; i >= 0; i--)
            {
                var slot = info.animslots[i];
                if (slot.active) slot.elapsed += tick;

                if (false == slot.istransient) continue;
                slot.duration -= tick;
                if (FP.Zero >= slot.duration)
                {
                    ReleaseSlot(info, slot);
                }
            }

            // 移除已结束的管线特效
            if (stage.SeekBehaviorInfos(out List<FlowEffectInfo> floweffects, true))
            {
                foreach (var floweffect in floweffects)
                {
                    if (floweffect.active) continue;
                    info.rmveffects.AddRange(floweffect.effects);
                }
            }

            // 移除过期的特效
            foreach (var rmveffect in info.rmveffects)
            {
                info.effectdict.Remove(rmveffect);
            }
            info.rmveffects.Clear();

            // 更新特效时间流逝
            var effectKeys = ObjectCache.Ensure<GBLList<uint>>();
            foreach (var kv in info.effectdict) effectKeys.Add(kv.Key);
            foreach (var id in effectKeys)
            {
                if (false == info.effectdict.TryGetValue(id, out var effect)) continue;
                effect.elapsed += tick;
                info.effectdict[id] = effect;
                if (effect.elapsed >= effect.duration) info.rmveffects.Add(id);
            }
            effectKeys.Dispose();
        }
    }

    protected override void OnEndTick() { }
}

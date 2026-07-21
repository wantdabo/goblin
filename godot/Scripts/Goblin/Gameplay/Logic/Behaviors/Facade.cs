using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 外观行为
/// </summary>
public class Facade : Behavior<FacadeInfo>
{
    /// <summary>
    /// 设置模型
    /// </summary>
    /// <param name="model">模型 ID</param>
    public void SetModel(int model)
    {
        info.model = model;
    }

    /// <summary>
    /// 设置动画状态
    /// </summary>
    /// <param name="state">动画状态</param>
    public void SetAnimation(byte state)
    {
        info.animstate = state;
        info.animhash = 0;
        info.animelapsed = 0;
        var priority = STATE_DEFINE.DEATH == state || STATE_DEFINE.BORN == state
            ? ANIM_DEFINE.SLOT_PRIORITY_LIFESTATE
            : ANIM_DEFINE.SLOT_PRIORITY_LOCOMOTION;
        AddOrUpdateSlot(ANIM_DEFINE.SLOT_STATE, priority, state: state);
        RmvSlot(ANIM_DEFINE.SLOT_OVERRIDE);
    }
        
    /// <summary>
    /// 设置动画名称
    /// </summary>
    /// <param name="animname">动画名称</param>
    public void SetAnimation(string animname, byte ticktype = ANIM_DEFINE.TICK_AUTOMATIC, byte layer = ANIM_DEFINE.LAYER_FULLBODY)
    {
        info.animticktype = ticktype;
        info.animhash = AnimHash.Hash(animname);
        info.animelapsed = 0;
        if (null != animname)
            AddOrUpdateSlot(ANIM_DEFINE.SLOT_NAMED, ANIM_DEFINE.SLOT_PRIORITY_ACTION, namehash: info.animhash, layer: layer);
        else
            RmvSlot(ANIM_DEFINE.SLOT_NAMED);
    }

    /// <summary>
    /// 添加或更新槽位
    /// </summary>
    public void AddOrUpdateSlot(byte key, int priority, byte state = 0, uint namehash = 0, byte layer = ANIM_DEFINE.LAYER_FULLBODY, FP duration = default)
    {
        var slot = GetSlot(key);
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
        if (FP.Zero < duration) { slot.istransient = true; slot.duration = duration; }
        else { slot.istransient = false; slot.duration = FP.Zero; }
        EnsureSort();
    }

    /// <summary>
    /// 移除槽位
    /// </summary>
    public void RmvSlot(byte key)
    {
        var slot = GetSlot(key);
        if (null != slot)
        {
            info.animslots.Remove(slot);
            slot.active = false;
            slot.animstate = 0;
            slot.animhash = 0;
            slot.layer = 0;
            slot.istransient = false;
            slot.duration = FP.Zero;
            ObjectCache.Set(slot);
        }
    }

    /// <summary>
    /// 按优先级降序排序
    /// </summary>
    private void EnsureSort() => info.animslots.Sort((a, b) => b.priority.CompareTo(a.priority));

    /// <summary>
    /// 查找槽位
    /// </summary>
    private AnimationSlot GetSlot(byte key)
    {
        foreach (var slot in info.animslots)
            if (slot.key == key) return slot;
        return null;
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="effect">特效</param>
    public uint CreateEffect(EffectInfo effect)
    {
        var increment = info.effectincrement++;
        effect.id = increment;
        effect.elapsed = 0;
        info.effects.Add(effect.id);
        info.effectdict.Add(effect.id, effect);
            
        return increment;
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (info.animticktype == ANIM_DEFINE.TICK_AUTOMATIC) info.animelapsed += tick;

        // 过期瞬时槽位
        for (int i = info.animslots.Count - 1; i >= 0; i--)
        {
            var slot = info.animslots[i];
            if (false == slot.istransient) continue;
            slot.duration -= tick;
            if (FP.Zero >= slot.duration)
            {
                info.animslots.RemoveAt(i);
                slot.active = false;
                slot.animstate = 0;
                slot.animhash = 0;
                slot.layer = 0;
                slot.istransient = false;
                slot.duration = FP.Zero;
                ObjectCache.Set(slot);
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
            if (info.effectdict.TryGetValue(rmveffect, out var effect)) DiffFacadeEffect(effect, RIL_DEFINE.DIFF_DEL);

            info.effects.Remove(rmveffect);
            info.effectdict.Remove(rmveffect);
        }
        info.rmveffects.Clear();

        // 更新特效时间流逝
        foreach (var id in info.effects)
        {
            if (false == info.effectdict.TryGetValue(id, out var effect)) continue;
            effect.elapsed += tick;
            info.effectdict.Remove(id);
            info.effectdict.Add(id, effect);
            if (effect.elapsed >= effect.duration) info.rmveffects.Add(id);
                
            DiffFacadeEffect(effect, RIL_DEFINE.DIFF_NEW);
        }
    }

    /// <summary>
    /// 外观特效差异
    /// </summary>
    /// <param name="effect">特效信息</param>
    /// <param name="token">RIL 差异标记</param>
    private void DiffFacadeEffect(EffectInfo effect, byte token)
    {
        var diff = ObjectCache.Ensure<RIL_DIFF_FACADE_EFFECT>();
        diff.Ready(actor, token);
        diff.effect = effect;
            
        stage.Diff(diff);
    }
}
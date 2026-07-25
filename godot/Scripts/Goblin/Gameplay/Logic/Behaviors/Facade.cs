using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
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
		AddOrUpdateSlot(ANIM_DEFINE.SLOT_TYPE_STATE, priority, state: state);
		RmvSlotsByType(ANIM_DEFINE.SLOT_TYPE_OVERRIDE);
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
			AddOrUpdateSlot(ANIM_DEFINE.SLOT_TYPE_NAMED, ANIM_DEFINE.SLOT_PRIORITY_ACTION, namehash: info.animhash, layer: layer);
		else
			RmvSlot(ANIM_DEFINE.GenKey(ANIM_DEFINE.SLOT_TYPE_NAMED, layer));
	}

	/// <summary>
	/// 添加或更新槽位
	/// </summary>
	public void AddOrUpdateSlot(byte slottype, int priority, byte state = 0, uint namehash = 0, byte layer = ANIM_DEFINE.LAYER_FULLBODY, FP duration = default)
	{
		var key = ANIM_DEFINE.GenKey(slottype, layer);
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
		slot.elapsed = FP.Zero;
		if (FP.Zero < duration) { slot.istransient = true; slot.duration = duration; }
		else { slot.istransient = false; slot.duration = FP.Zero; }
		EnsureSort();
	}

	/// <summary>
	/// 移除槽位
	/// </summary>
	public void RmvSlot(ushort key)
	{
		var slot = GetSlot(key);
		if (null != slot) ReleaseSlot(slot);
	}

	/// <summary>
	/// 按槽位类型移除所有匹配槽位
	/// </summary>
	public void RmvSlotsByType(byte slottype)
	{
		for (int i = info.animslots.Count - 1; i >= 0; i--)
		{
			if (ANIM_DEFINE.GetSlotType(info.animslots[i].key) != slottype) continue;
			ReleaseSlot(info.animslots[i]);
		}
	}

	/// <summary>
	/// 内部释放槽位（移出列表 + 重置字段 + 回池）
	/// </summary>
	private void ReleaseSlot(AnimationSlot slot)
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

	/// <summary>
	/// 按优先级降序排序
	/// </summary>
	private void EnsureSort() => info.animslots.Sort((a, b) => b.priority.CompareTo(a.priority));

	/// <summary>
	/// 查找槽位
	/// </summary>
	private AnimationSlot GetSlot(ushort key)
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
		info.effectdict.Add(effect.id, effect);
			
		return increment;
	}

	protected override void OnTick(FP tick)
	{
		base.OnTick(tick);
		if (info.animticktype == ANIM_DEFINE.TICK_AUTOMATIC) info.animelapsed += tick;

		// 逐槽位递进 elapsed（瞬时与非瞬时均需推进动画进度）
		for (int i = info.animslots.Count - 1; i >= 0; i--)
		{
			var slot = info.animslots[i];
			if (slot.active) slot.elapsed += tick;

			if (false == slot.istransient) continue;
			slot.duration -= tick;
			if (FP.Zero >= slot.duration)
			{
				ReleaseSlot(slot);
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
		// 复制键列表，避免在遍历时修改 effectdict
		var effectKeys = ObjectCache.Ensure<List<uint>>();
		foreach (var kv in info.effectdict) effectKeys.Add(kv.Key);
		foreach (var id in effectKeys)
		{
			if (false == info.effectdict.TryGetValue(id, out var effect)) continue;
			effect.elapsed += tick;
			info.effectdict[id] = effect;
			if (effect.elapsed >= effect.duration) info.rmveffects.Add(id);
		}
		effectKeys.Clear();
		ObjectCache.Set(effectKeys);
	}
}

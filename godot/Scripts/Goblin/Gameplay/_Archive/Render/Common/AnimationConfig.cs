using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Render.Common;

public class AnimationConfig
{
	public int model { get; set; }
	public List<AnimationStateInfo> animationstates { get; set; }
	public List<AnimationMixInfo> animationmixs { get; set; }

	/// <summary>
	/// AnimationTree 各层 blend node 名称（Base/Upper/Lower），可在配置中覆盖
	/// </summary>
	public List<string> layernodenames { get; set; }

	/// <summary>
	/// 默认层节点名称兜底
	/// </summary>
	private static readonly string[] defaultlayernodenames = { "Base", "Upper", "Lower" };

	/// <summary>
	/// 获取指定层的 blend node 名称
	/// </summary>
	public string GetLayerNodeName(int layer)
	{
		if (null != layernodenames && layer < layernodenames.Count)
			return layernodenames[layer];
		if (layer < defaultlayernodenames.Length)
			return defaultlayernodenames[layer];
		return null;
	}

	private Dictionary<uint, string> hash2name { get; set; }
	private Dictionary<uint, AnimationMixInfo> hash2mixinfo { get; set; }
	private Dictionary<byte, string> state2name { get; set; }

	/// <summary>
	/// 构建哈希索引（加载配置后调用一次）
	/// </summary>
	public void BuildHashIndex()
	{
		if (null != animationstates)
		{
			hash2name = new Dictionary<uint, string>(animationstates.Count);
			state2name = new Dictionary<byte, string>(animationstates.Count);
			foreach (var s in animationstates)
			{
				hash2name[AnimHash.Hash(s.name)] = s.name;
				state2name[s.state] = s.name;
			}
		}

		if (null != animationmixs)
		{
			hash2mixinfo = new Dictionary<uint, AnimationMixInfo>(animationmixs.Count);
			foreach (var m in animationmixs)
				hash2mixinfo[AnimHash.Hash(m.name)] = m;
		}
	}

	public string GetAnimationName(byte state)
	{
		if (null == state2name) return null;
		state2name.TryGetValue(state, out var name);
		return name;
	}

	/// <summary>
	/// 通过哈希查找动画名称
	/// </summary>
	public string GetAnimationNameByHash(uint hash)
	{
		if (null == hash2name || 0 == hash) return null;
		hash2name.TryGetValue(hash, out var name);
		return name;
	}

	public AnimationMixInfo GetAnimationMixInfo(string name)
	{
		foreach (var m in animationmixs)
			if (m.name == name) return m;
		return null;
	}

	/// <summary>
	/// 通过哈希查找动画混合信息
	/// </summary>
	public AnimationMixInfo GetAnimationMixInfoByHash(uint hash)
	{
		if (null == hash2mixinfo || 0 == hash) return null;
		hash2mixinfo.TryGetValue(hash, out var info);
		return info;
	}
}

[Serializable]
public class AnimationStateInfo
{
	public byte state { get; set; }
	public string name { get; set; }
}

[Serializable]
public class AnimationMixInfo
{
	public string name { get; set; }
	public float mixduration { get; set; }
	public List<AnimationBeforeMixInfo> mixanimations { get; set; }

	public AnimationBeforeMixInfo GetAnimationBeforeMixInfo(string name)
	{
		foreach (var a in mixanimations)
			if (a.prename == name) return a;
		return null;
	}
}

[Serializable]
public class AnimationBeforeMixInfo
{
	public string prename { get; set; }
	public string name { get; set; }
	public float duration { get; set; }
	public float mixduration { get; set; }
}

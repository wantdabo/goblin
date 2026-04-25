using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Render.Common;

public static class AnimationConfigCache
{
	public static AnimationConfig current { get; set; }
}

public class AnimationConfig
{
	public int model { get; set; }
	public List<AnimationStateInfo> animationstates { get; set; }
	public List<AnimationMixInfo> animationmixs { get; set; }

	public string GetAnimationName(byte state)
	{
		foreach (var s in animationstates)
			if (s.state == state) return s.name;
		return null;
	}

	public AnimationMixInfo GetAnimationMixInfo(string name)
	{
		foreach (var m in animationmixs)
			if (m.name == name) return m;
		return null;
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

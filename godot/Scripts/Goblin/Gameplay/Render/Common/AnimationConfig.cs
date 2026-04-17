using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Render.Common
{
    public static class AnimationConfigCache
    {
        public static AnimationConfig current { get; set; }
    }

    public class AnimationConfig
    {
        public int model;
        public List<AnimationStateInfo> animationstates;
        public List<AnimationMixInfo> animationmixs;

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
        public byte state;
        public string name;
    }

    [Serializable]
    public class AnimationMixInfo
    {
        public string name;
        public float mixduration;
        public List<AnimationBeforeMixInfo> mixanimations;

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
        public string prename;
        public string name;
        public float duration;
        public float mixduration;
    }
}

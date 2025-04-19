using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    [CreateAssetMenu(fileName = "AnimationSettings", menuName = "创建动画配置文件")]
    public class AnimationConfig : ScriptableObject
    {
        public List<AnimationInfo> animations;
        
        public AnimationInfo GetAnimationInfo(byte state)
        {
            foreach (var animation in animations)
            {
                if (animation.state == state)
                {
                    return animation;
                }
            }
            return null;
        }
    }
    
    [Serializable]
    public class AnimationInfo
    {
        public byte state;
        public string name;
        public float mixduration;
        public List<AnimationBeforeInfo> mixanimations;
        
        public AnimationBeforeInfo GetMixAnimationInfo(byte state)
        {
            foreach (var animation in mixanimations)
            {
                if (animation.state == state)
                {
                    return animation;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class AnimationBeforeInfo
    {
        public byte state;
        public string name;
        public float duration;
        public float mixduration;
    }
}
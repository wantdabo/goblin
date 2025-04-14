using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimationSettings", menuName = "创建动画配置文件")]
    public class AnimationConfig : ScriptableObject
    {
        public List<AnimationInfo> animations;
    }

    [Serializable]
    public class AnimationInfo
    {
        public byte state;
        public string name;
        public float tarduration;
        public float mixduration;
        public List<(byte state, string name, float tarduration, float mixduration)> mixanimations;
    }
}
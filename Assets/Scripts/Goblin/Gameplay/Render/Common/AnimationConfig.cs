using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 动画配置
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationSettings", menuName = "创建动画配置文件")]
    public class AnimationConfig : ScriptableObject
    {
        /// <summary>
        /// 动画配置信息列表
        /// </summary>
        [LabelText("动画配置信息列表")]
        [TableMatrix]
        public List<AnimationInfo> animations;
        
        /// <summary>
        /// 获取动画配置信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>动画配置信息</returns>
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
    
    /// <summary>
    /// 动画配置信息
    /// </summary>
    [Serializable]
    public class AnimationInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        [LabelText("状态")]
        [ValueDropdown("@OdinDefineValueDropdownList.GetStateDefine()")]
        public byte state;
        /// <summary>
        /// 动画名称
        /// </summary>
        [LabelText("动画名称")]
        public string name;
        /// <summary>
        /// 动画混合时间
        /// </summary>
        [LabelText("动画混合时间")]
        public float mixduration;
        /// <summary>
        /// 动画前置混合动画列表
        /// </summary>
        [LabelText("动画前置混合动画列表")]
        [TableMatrix]
        public List<AnimationBeforeInfo> mixanimations;
        
        /// <summary>
        /// 获取前置混合动画信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>前置混合动画信息</returns>
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

    /// <summary>
    /// 前置混合动画信息
    /// </summary>
    [Serializable]
    public class AnimationBeforeInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        [LabelText("状态")]
        [ValueDropdown("@OdinDefineValueDropdownList.GetStateDefine()")]
        public byte state;
        /// <summary>
        /// 动画名称
        /// </summary>
        [LabelText("动画名称")]
        public string name;
        /// <summary>
        /// 动画持续时间
        /// </summary>
        [LabelText("动画持续时间")]
        public float duration;
        /// <summary>
        /// 动画混合时间
        /// </summary>
        [LabelText("动画混合时间")]
        public float mixduration;
    }
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 动画配置缓存
    /// </summary>
    public static class AnimationConfigCache
    {
        /// <summary>
        /// 当前动画配置
        /// </summary>
        public static AnimationConfig current { get; set; }
    }

    /// <summary>
    /// 动画配置
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationConfig", menuName = "创建动画配置文件")]
    public class AnimationConfig : ScriptableObject
    {
        /// <summary>
        /// 模型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.ModelValueDropdown()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择模型")]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("模型")]
        public int model;
        
        /// <summary>
        /// 动画状态绑定信息列表
        /// </summary>
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("动画状态绑定信息")]
        public List<AnimationStateInfo> animationstates;
        
        /// <summary>
        /// 动画配置列表
        /// </summary>
        [LabelText("动画配置列表")]
        [TableMatrix]
        public List<AnimationMixInfo> animationmixs;
        
        /// <summary>
        /// 获取动画名称
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>动画名称</returns>
        public string GetAnimationName(byte state)
        {
            foreach (var animstate in animationstates)
            {
                if (animstate.state != state) continue;
                return animstate.name;
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取动画配置信息
        /// </summary>
        /// <param name="name">动画名称</param>
        /// <returns>动画配置信息</returns>
        public AnimationMixInfo GetAnimationMixInfo(string name)
        {
            foreach (var animmix in animationmixs)
            {
                if (animmix.name != name) continue;
                
                return animmix;
            }
            
            return null;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            AnimationConfigCache.current = this;
        }
#endif
    }

    /// <summary>
    /// 动画状态绑定信息
    /// </summary>
    [Serializable]
    public class AnimationStateInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        [LabelText("状态")]
        [ValueDropdown("@OdinValueDropdown.GetStateDefine()")]
        public byte state;
        /// <summary>
        /// 动画名称
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetModelAnimNames(AnimationConfigCache.current.model)")]
        [LabelText("动画名称")]
        public string name;
    }
    
    /// <summary>
    /// 动画配置信息
    /// </summary>
    [Serializable]
    public class AnimationMixInfo
    {
        /// <summary>
        /// 动画名称
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetModelAnimNames(AnimationConfigCache.current.model)")]
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
        public List<AnimationBeforeMixInfo> mixanimations;
        
        /// <summary>
        /// 获取前置混合动画信息
        /// </summary>
        /// <param name="name">动画名称</param>
        /// <returns>前置混合动画信息</returns>
        public AnimationBeforeMixInfo GetAnimationBeforeMixInfo(string name)
        {
            foreach (var animation in mixanimations)
            {
                if (animation.prename == name)
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
    public class AnimationBeforeMixInfo
    {
        /// <summary>
        /// 上一个动画名称
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetModelAnimNames(AnimationConfigCache.current.model)")]
        [LabelText("上一个动画名称")]
        public string prename;
        /// <summary>
        /// 动画名称
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetModelAnimNames(AnimationConfigCache.current.model)")]
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
using Animancer;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Render.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Goblin.Misc
{
    /// <summary>
    /// Odin 定义值下拉列表
    /// </summary>
    public class OdinValueDropdown
    {
        /// <summary>
        /// 获取状态定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(状态)</returns>
        public static ValueDropdownList<byte> GetStateDefine()
        {
            return new ()
            {
                { "待机", STATE_DEFINE.IDLE },
                { "移动", STATE_DEFINE.MOVE },
                { "跳跃", STATE_DEFINE.JUMP },
                { "下坠", STATE_DEFINE.FALL },
                { "技能", STATE_DEFINE.CASTING },
            };
        }
        
        /// <summary>
        /// 获取特效类型定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(特效类型)</returns>
        public static ValueDropdownList<byte> GetEffectTypeDefine()
        {
            return new ()
            {
                { "标准特效", EFFECT_DEFINE.TYPE_STANDAR },
                { "线条特效", EFFECT_DEFINE.TYPE_LINE },
            };
        }
        
        /// <summary>
        /// 获取特效跟随定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(特效跟随)</returns>
        public static ValueDropdownList<byte> GetEffectFollowDefine()
        {
            return new ()
            {
                { "跟随 Actor", EFFECT_DEFINE.FOLLOW_ACTOR },
                { "跟随挂点", EFFECT_DEFINE.FOLLOW_MOUNT },
            };
        }
        
        /// <summary>
        /// 获取碰撞类型定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(碰撞检测类型)</returns>
        public static ValueDropdownList<byte> GetCollisionTypeDefine()
        {
            return new ()
            {
                { "攻击盒", COLLISION_DEFINE.COLLISION_TYPE_HURT },
                { "嗅探器", COLLISION_DEFINE.COLLISION_TYPE_SENSOR },
            };
        }
        
        /// <summary>
        /// 获取碰撞检测类型定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(碰撞检测类型)</returns>
        public static ValueDropdownList<byte> GetCollisionOverlapDefine()
        {
            return new ()
            {
                { "射线检测", COLLISION_DEFINE.COLLISION_RAY },
                { "线段检测", COLLISION_DEFINE.COLLISION_LINE },
                { "立方体检测", COLLISION_DEFINE.COLLISION_BOX },
                { "球体检测", COLLISION_DEFINE.COLLISION_SPHERE },
            };
        }
        
        /// <summary>
        /// 获取特效跟随掩码定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(特效跟随掩码)</returns>
        public static ValueDropdownList<int> GetEffectFollowMaskDefine()
        {
            return new ()
            {
                { "None", EFFECT_DEFINE.FOLLOW_NONE },
                { "Position", EFFECT_DEFINE.FOLLOW_POSITION },
                { "Rotation", EFFECT_DEFINE.FOLLOW_ROTATION },
                { "Scale", EFFECT_DEFINE.FOLLOW_SCALE },
                { "Position + Rotation + Scale", EFFECT_DEFINE.FOLLOW_POSITION | EFFECT_DEFINE.FOLLOW_ROTATION | EFFECT_DEFINE.FOLLOW_SCALE },
                { "Position + Rotation", EFFECT_DEFINE.FOLLOW_POSITION | EFFECT_DEFINE.FOLLOW_ROTATION },
                { "Position + Scale", EFFECT_DEFINE.FOLLOW_POSITION | EFFECT_DEFINE.FOLLOW_SCALE },
                { "Rotation + Scale", EFFECT_DEFINE.FOLLOW_ROTATION | EFFECT_DEFINE.FOLLOW_SCALE },
            };
        }
        
        /// <summary>
        /// 获取输入定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(输入 ID)</returns>
        public static ValueDropdownList<ushort> GetInputDefine()
        {
            return new ()
            {
                { "Joystick", INPUT_DEFINE.JOYSTICK },
                { "BA", INPUT_DEFINE.BA },
                { "BB", INPUT_DEFINE.BB },
                { "BC", INPUT_DEFINE.BC },
                { "BD", INPUT_DEFINE.BD },
            };
        }

        /// <summary>
        /// 获取条件定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(条件 ID)</returns>
        public static ValueDropdownList<ushort> GetConditionDefine()
        {
            return new ()
            {
                { "输入条件", CONDITION_DEFINE.INPUT },
            };
        }
        
        /// <summary>
        /// 获取状态定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(状态)</returns>
        public static ValueDropdownList<byte> GetSpatialPositionDefine()
        {
            return new ()
            {
                { "世界参考", SPATIAL_DEFINE.POSITION_WORLD },
                { "自身参考", SPATIAL_DEFINE.POSITION_SELF },
            };
        }
        
        /// <summary>
        /// 获取模型下拉列表
        /// </summary>
        /// <returns>模型下拉列表</returns>
        public static ValueDropdownList<int> ModelValueDropdown()
        {
            var result = new ValueDropdownList<int>();
            result.Add(new ValueDropdownItem<int>("None", 0));
            foreach (var modelcfg in EditorConfig.location.ModelInfos.DataList) result.Add(new ValueDropdownItem<int>(modelcfg.Res, modelcfg.Id));
            
            return result;
        }

        /// <summary>
        /// 获取特效下拉列表
        /// </summary>
        /// <returns>特效下拉列表</returns>
        public static ValueDropdownList<int> EffectValueDropdown()
        {
            var result = new ValueDropdownList<int>();
            result.Add(new ValueDropdownItem<int>("None", 0));
            foreach (var effectcfg in EditorConfig.location.EffectInfos.DataList) result.Add(new ValueDropdownItem<int>($"{effectcfg.Id} - {effectcfg.Res}", effectcfg.Id));
            
            return result;
        }
        
        /// <summary>
        /// 获取模型下拉列表
        /// </summary>
        /// <returns>模型下拉列表</returns>
        public static ValueDropdownList<uint> SKillIds()
        {
            var result = new ValueDropdownList<uint>();
            result.Add(new ValueDropdownItem<uint>("None", 0));
            foreach (var skillcfg in EditorConfig.location.SkillInfos.DataList)
            {
                result.Add(new ValueDropdownItem<uint>($"{skillcfg.Id} - {skillcfg.Name}", (uint)skillcfg.Id));
            }
            
            return result;
        }

        /// <summary>
        /// 获取模型动画名称列表
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>模型动画名称列表</returns>
        public static ValueDropdownList<string> GetModelAnimNames(int model)
        {
            var result = new ValueDropdownList<string>();
            if (false == EditorConfig.location.ModelInfos.TryGetValue(model, out var modelcfg)) return result;
            var go = EditorRes.LoadModel(modelcfg.Res);
            var animancer = go.GetComponent<NamedAnimancerComponent>();
            if (null == animancer)
            {
                GameObject.DestroyImmediate(go);
                return result;
            }
            
            foreach (var clip in animancer.Animations) result.Add(clip.name, clip.name);
            
            GameObject.DestroyImmediate(go);
            
            return result;
        }
    }
}
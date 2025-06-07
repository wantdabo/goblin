using Animancer;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Render.Common;
using Sirenix.OdinInspector;
using UnityEditor;

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
            if (null == animancer) return result;
            
            foreach (var clip in animancer.Animations) result.Add(clip.name, clip.name);
            
            return result;
        }
    }
}
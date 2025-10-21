using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Pipeline.Timeline.Common
{
    /// <summary>
    /// 管线火花指令包装
    /// </summary>
    [Serializable]
    public class PipelineSparkInstruct
    {
        [LabelText("火花触发范围")]
        [ValueDropdown("@OdinValueDropdown.GetSparkInfluenceDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "触发范围")] 
        public sbyte influence;
        
        [HideLabel]
        public string token {
            get
            {
                return useinnertoken ? innertoken : customtoken;
            }
        }

        [LabelText("使用内置令牌")]
        public bool useinnertoken = true;
        
        [LabelText("火花令牌")] 
        [ShowIf("@true == useinnertoken")]
        [ValueDropdown("@OdinValueDropdown.GetSparkTokenDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "内置火花令牌")] 
        public string innertoken = SPARK_INSTR_DEFINE.TOKEN_IMMEDIATE;

        [ShowIf("@false == useinnertoken")]
        [LabelText("火花令牌")]
        public string customtoken;

        [LabelText("使用火花令牌变体")]
        public bool usetokenvariant;

        [ShowIf("@true == usetokenvariant")]
        [LabelText("火花令牌变体")]
        public string tokenvariant;

        [LabelText("条件列表")]
        [PropertySpace(SpaceAfter = 5)]
        public List<PipelineCondition> conditions;

        [ValueDropdown("@OdinValueDropdown.GetSparkInstructDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "指令类型")]
        [HideLabel]
        public ushort instrtype = INSTR_DEFINE.SPATIAL_POSITION;

        [ShowIf("@INSTR_DEFINE.SPATIAL_POSITION == instrtype")]
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public SpatialPositionData spatialpositioninstr;

        [ShowIf("@INSTR_DEFINE.LAUNCH_SKILL == instrtype")]
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public LaunchSkillData launchskillinstr;

        [ShowIf("@INSTR_DEFINE.EFFECT == instrtype")]
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public EffectData effectinstr;

        [ShowIf("@INSTR_DEFINE.COLLISION == instrtype")] 
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public CollisionData collisioninstr;

        [ShowIf("@INSTR_DEFINE.RMV_ACTOR == instrtype")]
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public RmvActorData rmvactorinstr;

        [ShowIf("@INSTR_DEFINE.CHANGE_STATE == instrtype")]
        [HideLabel]
        [PropertySpace(SpaceAfter = 5)]
        public ChangeStateData changestateinstr;

        /// <summary>
        /// 获取指令数据
        /// </summary>
        /// <returns>指令数据</returns>
        /// <exception cref="NotImplementedException">未找到指令数据</exception>
        public InstructData GetInstructData()
        {
            switch (instrtype)
            {
                case INSTR_DEFINE.SPATIAL_POSITION:
                    return spatialpositioninstr;
                case INSTR_DEFINE.LAUNCH_SKILL:
                    return launchskillinstr;
                case INSTR_DEFINE.EFFECT:
                    return effectinstr;
                case INSTR_DEFINE.COLLISION:
                    return collisioninstr;
                case INSTR_DEFINE.RMV_ACTOR:
                    return rmvactorinstr;
                case INSTR_DEFINE.CHANGE_STATE:
                    return changestateinstr;
                default:
                    throw new NotImplementedException($"instr with type {instrtype} not implemented.");
            }
        }

        /// <summary>
        /// 设置指令数据
        /// </summary>
        /// <param name="data">指令数据</param>
        /// <exception cref="NotImplementedException">未能正确设置指令数据</exception>
        public void SetInstructData(InstructData data)
        {
            switch (instrtype)
            {
                case INSTR_DEFINE.SPATIAL_POSITION:
                    spatialpositioninstr = data as SpatialPositionData;
                    break;
                case INSTR_DEFINE.LAUNCH_SKILL:
                    launchskillinstr = data as LaunchSkillData;
                    break;
                case INSTR_DEFINE.EFFECT:
                    effectinstr = data as EffectData;
                    break;
                case INSTR_DEFINE.COLLISION:
                    collisioninstr = data as CollisionData;
                    break;
                case INSTR_DEFINE.RMV_ACTOR:
                    rmvactorinstr = data as RmvActorData;
                    break;
                case INSTR_DEFINE.CHANGE_STATE:
                    changestateinstr = data as ChangeStateData;
                    break;
                default:
                    throw new NotImplementedException($"instr with type {instrtype} not implemented.");
            }
        }
    }
}
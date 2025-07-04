﻿using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows
{
    /// <summary>
    /// 管线数据序列化工具类
    /// </summary>
    public static class PipelineDataSerialize
    {
        /// <summary>
        /// 将管线数据转换为原始数据格式
        /// </summary>
        /// <param name="data">PipelineData</param>
        /// <returns>PipelineRawData</returns>
        public static PipelineRawData ToPipelineRawData(this PipelineData data)
        {
            var raw = new PipelineRawData();
            raw.length = data.length;
            raw.begin = new ulong[data.instructs.Count];
            raw.end = new ulong[data.instructs.Count];
            raw.checkonce = new bool[data.instructs.Count];
            raw.instrtypes = new ushort[data.instructs.Count];
            raw.instrdata = new byte[data.instructs.Count][];
            raw.conditiontypes = new ushort[data.instructs.Count][];
            raw.conditions = new byte[data.instructs.Count][][];
            
            for (int i = 0; i < data.instructs.Count; i++)
            {
                var instruct = data.instructs[i];
                raw.begin[i] = instruct.begin;
                raw.end[i] = instruct.end;
                raw.checkonce[i] = instruct.checkonce;
                raw.instrtypes[i] = instruct.data.id;
                raw.instrdata[i] = instruct.data.Serialize();
                
                raw.conditiontypes[i] = new ushort[instruct.conditions.Count];
                raw.conditions[i] = new byte[instruct.conditions.Count][];
                for (int j = 0; j < instruct.conditions.Count; j++)
                {
                    var condition = instruct.conditions[j];
                    raw.conditiontypes[i][j] = condition.id;
                    raw.conditions[i][j] = condition.Serialize();
                }
            }

            return raw;
        }
        
        /// <summary>
        /// 将原始管线数据转换为管线数据格式
        /// </summary>
        /// <param name="rawdata">PipelineRawData</param>
        /// <returns>PipelineData</returns>
        public static PipelineData ToPipelineData(this PipelineRawData rawdata)
        {
            var data = new PipelineData
            {
                length = rawdata.length,
                instructs = new List<Instruct>()
            };

            for (int i = 0; i < rawdata.instrtypes.Length; i++)
            {
                var instrtype = rawdata.instrtypes[i];
                var instruct = new Instruct
                {
                    begin = rawdata.begin[i],
                    end = rawdata.end[i],
                    checkonce = rawdata.checkonce[i],
                };
                
                switch (instrtype)
                {
                    case INSTR_DEFINE.ANIMATION:
                        instruct.data = MessagePackSerializer.Deserialize<AnimationData>(rawdata.instrdata[i]);
                        break;
                    case INSTR_DEFINE.SPATIAL_POSITION:
                        instruct.data = MessagePackSerializer.Deserialize<SpatialPositionData>(rawdata.instrdata[i]);
                        break;
                    case INSTR_DEFINE.LAUNCH_SKILL:
                        instruct.data = MessagePackSerializer.Deserialize<LaunchSkillData>(rawdata.instrdata[i]);
                        break;
                    case INSTR_DEFINE.EFFECT:
                        instruct.data = MessagePackSerializer.Deserialize<EffectData>(rawdata.instrdata[i]);
                        break;
                    case INSTR_DEFINE.COLLISION:
                        instruct.data = MessagePackSerializer.Deserialize<CollisionData>(rawdata.instrdata[i]);
                        break;
                }

                instruct.conditions = new List<Condition>();
                for (int j = 0; j < rawdata.conditiontypes[i].Length; j++)
                {
                    var conditiontype = rawdata.conditiontypes[i][j];
                    Condition condition = default;
                    switch (conditiontype)
                    {
                        case CONDITION_DEFINE.INPUT:
                            condition = MessagePackSerializer.Deserialize<InputCondition>(rawdata.conditions[i][j]);
                            break;
                    }
                    instruct.conditions.Add(condition);
                }

                data.instructs.Add(instruct);
            }

            return data;
        }
    }
}
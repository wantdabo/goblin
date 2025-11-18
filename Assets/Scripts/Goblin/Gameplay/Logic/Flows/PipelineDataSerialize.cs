using System.Collections.Generic;
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
            
            // 初始化指令相关数据
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

            // 初始化火花相关数据
            raw.sparkinfluences = new sbyte[data.sparkinstructs.Count];
            raw.sparktoken = new string[data.sparkinstructs.Count];
            raw.sparkinstrtypes = new ushort[data.sparkinstructs.Count];
            raw.sparkinstrdata = new byte[data.sparkinstructs.Count][];
            raw.sparkconditiontypes = new ushort[data.sparkinstructs.Count][];
            raw.sparkconditions = new byte[data.sparkinstructs.Count][][];
            for (int i = 0; i < data.sparkinstructs.Count; i++)
            {
                var instruct = data.sparkinstructs[i];
                raw.sparkinfluences[i] = instruct.influence;
                raw.sparktoken[i] = instruct.token;
                raw.sparkinstrtypes[i] = instruct.data.id;
                raw.sparkinstrdata[i] = instruct.data.Serialize();
                
                raw.sparkconditiontypes[i] = new ushort[instruct.conditions.Count];
                raw.sparkconditions[i] = new byte[instruct.conditions.Count][];
                for (int j = 0; j < instruct.conditions.Count; j++)
                {
                    var condition = instruct.conditions[j];
                    raw.sparkconditiontypes[i][j] = condition.id;
                    raw.sparkconditions[i][j] = condition.Serialize();
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
                instructs = new List<Instruct>(),
                sparkinstructs = new List<SparkInstruct>(),
            };

            if (null != rawdata.instrtypes)
            {
                for (int i = 0; i < rawdata.instrtypes.Length; i++)
                {
                    var instrtype = rawdata.instrtypes[i];
                    var instruct = new Instruct
                    {
                        begin = rawdata.begin[i],
                        end = rawdata.end[i],
                        checkonce = rawdata.checkonce[i],
                        conditions = new List<Condition>()
                    };

                    instruct.data = BytesToInstructData(instrtype, rawdata.instrdata[i]);
                    for (int j = 0; j < rawdata.conditiontypes[i].Length; j++)
                    {
                        var conditiontype = rawdata.conditiontypes[i][j];
                        Condition condition = BytesToCondition(conditiontype, rawdata.conditions[i][j]);
                        instruct.conditions.Add(condition);
                    }

                    data.instructs.Add(instruct);
                }
            }

            if (null != rawdata.sparkinstrtypes)
            {
                // 处理火花指令
                for (int i = 0; i < rawdata.sparkinstrtypes.Length; i++)
                {
                    var instrtype = rawdata.sparkinstrtypes[i];
                    var sparkinstruct = new SparkInstruct
                    {
                        influence = rawdata.sparkinfluences[i],
                        token = rawdata.sparktoken[i],
                        conditions = new List<Condition>(),
                    };

                    sparkinstruct.data = BytesToInstructData(instrtype, rawdata.sparkinstrdata[i]);
                    sparkinstruct.conditions = new List<Condition>();
                    for (int j = 0; j < rawdata.sparkconditiontypes[i].Length; j++)
                    {
                        var conditiontype = rawdata.sparkconditiontypes[i][j];
                        Condition condition = BytesToCondition(conditiontype, rawdata.sparkconditions[i][j]);
                        sparkinstruct.conditions.Add(condition);
                    }

                    data.sparkinstructs.Add(sparkinstruct);
                }
            }


            return data;
        }
        
        /// <summary>
        /// 将字节数组转换为指令数据
        /// </summary>
        /// <param name="instrtype">指令 ID</param>
        /// <param name="data">字节数组</param>
        /// <returns>指令数据</returns>
        private static InstructData BytesToInstructData(ushort instrtype, byte[] data)
        {
            switch (instrtype)
            {
                case INSTR_DEFINE.ANIMATION:
                    return MessagePackSerializer.Deserialize<AnimationData>(data);
                case INSTR_DEFINE.SPATIAL_POSITION:
                    return MessagePackSerializer.Deserialize<SpatialPositionData>(data);
                case INSTR_DEFINE.LAUNCH_SKILL:
                    return MessagePackSerializer.Deserialize<LaunchSkillData>(data);
                case INSTR_DEFINE.EFFECT:
                    return MessagePackSerializer.Deserialize<EffectData>(data);
                case INSTR_DEFINE.COLLISION:
                    return MessagePackSerializer.Deserialize<CollisionData>(data);
                case INSTR_DEFINE.RMV_ACTOR:
                    return MessagePackSerializer.Deserialize<RmvActorData>(data);
                case INSTR_DEFINE.CHANGE_STATE:
                    return MessagePackSerializer.Deserialize<ChangeStateData>(data);
                case INSTR_DEFINE.SPARK:
                    return MessagePackSerializer.Deserialize<SparkData>(data);
                case INSTR_DEFINE.HIT_LAG:
                    return MessagePackSerializer.Deserialize<HitLagData>(data);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 将字节数组转换为条件
        /// </summary>
        /// <param name="conditiontype">条件类型</param>
        /// <param name="data">字节数据</param>
        /// <returns>条件</returns>
        private static Condition BytesToCondition(ushort conditiontype, byte[] data)
        {
            switch (conditiontype)
            {
                case CONDITION_DEFINE.INPUT:
                    return MessagePackSerializer.Deserialize<InputCondition>(data);
                default:
                    return null;
            }
        }
    }
}
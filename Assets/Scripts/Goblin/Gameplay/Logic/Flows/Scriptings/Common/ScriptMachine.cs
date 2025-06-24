using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings.Common
{
    /// <summary>
    /// 脚本状态机
    /// </summary>
    public class ScriptMachine
    {
        /// <summary>
        /// 管线数据
        /// </summary>
        private static PipelineData pipelinedata { get; set; }

        /// <summary>
        /// 开始脚本状态机
        /// </summary>
        /// <exception cref="Exception">脚本状态机已经在运行中</exception>
        public static void Begin()
        {
            if (null != pipelinedata) throw new Exception("scriptmachine is already running.");

            pipelinedata = new PipelineData
            {
                length = 0,
                instructs = new List<Instruct>()
            };
        }

        /// <summary>
        /// 结束脚本状态机
        /// </summary>
        /// <returns>管线数据</returns>
        /// <exception cref="Exception">脚本状态机还未启动</exception>
        public static PipelineData End()
        {
            if (null == pipelinedata) throw new Exception("scriptmachine is not running.");

            pipelinedata.Format();
            var data = pipelinedata;
            pipelinedata = null;

            return data;
        }

        /// <summary>
        /// 插入指令
        /// </summary>
        /// <param name="begin">区间开始时间</param>
        /// <param name="end">区间结束时间</param>
        /// <param name="data">指令数据</param>
        /// <param name="checkonce">是否只检查一次</param>
        /// <typeparam name="T">指令数据类型</typeparam>
        /// <returns>脚本状态机操作器</returns>
        public static InstructOperation Instruct<T>(ulong begin, ulong end, T data, bool checkonce = true) where T : InstructData
        {
            Instruct instruct = new()
            {
                begin = begin,
                end = end,
                checkonce = checkonce,
                conditions = new(),
                data = data
            };
            pipelinedata.instructs.Add(instruct);

            return new InstructOperation(instruct);
        }

        /// <summary>
        /// 指令操作器
        /// </summary>
        public struct InstructOperation
        {
            /// <summary>
            /// 指令
            /// </summary>
            public Instruct instruct { get; private set; }

            /// <summary>
            /// 指令操作器
            /// </summary>
            /// <param name="instruct">指令</param>
            public InstructOperation(Instruct instruct)
            {
                this.instruct = instruct;
            }

            /// <summary>
            /// 添加条件
            /// </summary>
            /// <param name="condition">条件</param>
            /// <typeparam name="T">条件类型</typeparam>
            /// <returns>指令操作器</returns>
            public InstructOperation Condition<T>(T condition) where T : Condition
            {
                instruct.conditions.Add(condition);

                return this;
            }

            /// <summary>
            /// 添加条件
            /// </summary>
            /// <param name="conditions">条件列表</param>
            /// <returns>指令操作器</returns>
            public InstructOperation Condition(List<Condition> conditions)
            {
                foreach (var condition in conditions)
                {
                    instruct.conditions.Add(condition);
                }

                return this;
            }

            /// <summary>
            /// 在这个指令的后面插入指令
            /// </summary>
            /// <param name="offset">时间偏移</param>
            /// <param name="length">时间长度</param>
            /// <param name="data">指令数据</param>
            /// <param name="checkonce">是否只检查一次</param>
            /// <typeparam name="T">指令数据类型</typeparam>
            /// <returns>指令操作器</returns>
            public InstructOperation After<T>(ulong offset, ulong length, T data, bool checkonce = true) where T : InstructData
            {
                return Instruct(instruct.end + offset, instruct.end + length, data, checkonce);
            }
        }
    }
}
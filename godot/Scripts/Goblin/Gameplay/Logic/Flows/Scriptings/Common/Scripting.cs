using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings.Common
{
    /// <summary>
    /// 脚本指令
    /// </summary>
    public abstract class Scripting
    {
        /// <summary>
        /// 脚本 ID
        /// </summary>
        public abstract uint id { get; }

        /// <summary>
        /// 插入指令
        /// </summary>
        /// <param name="begin">区间开始时间</param>
        /// <param name="end">区间结束时间</param>
        /// <param name="data">指令数据</param>
        /// <typeparam name="T">指令数据类型</typeparam>
        /// <returns>脚本状态机操作器</returns>
        protected ScriptMachine.InstructOperation Instruct<T>(ulong begin, ulong end, T data) where T : InstructData
        {
            return ScriptMachine.Instruct<T>(begin, end, data);
        }

        /// <summary>
        /// 脚本指令加工 (构造管线数据)
        /// </summary>
        /// <returns></returns>
        public PipelineData Script()
        {
            ScriptMachine.Begin();
            OnScript();
            return ScriptMachine.End();
        }

        /// <summary>
        /// 脚本指令加工 (构造管线数据)
        /// </summary>
        protected abstract void OnScript();
    }
}
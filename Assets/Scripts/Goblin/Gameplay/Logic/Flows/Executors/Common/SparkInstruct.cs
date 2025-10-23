using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Common
{
    /// <summary>
    /// 火花指令
    /// </summary>
    public sealed class SparkInstruct
    {
        /// <summary>
        /// 火花触发范围
        /// </summary>
        public sbyte influence { get; set; }
        /// <summary>
        /// 火花令牌
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 执行条件列表
        /// </summary>
        public List<Condition> conditions { get; set; }
        /// <summary>
        /// 指令数据
        /// </summary>
        public InstructData data { get; set; }
    }
}
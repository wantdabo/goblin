using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Common
{
    /// <summary>
    /// 管线指令
    /// </summary>
    [MessagePackObject(true)]
    public sealed class Instruct
    {
        /// <summary>
        /// 区间开始
        /// </summary>
        public ulong begin { get; set; }
        /// <summary>
        /// 区间结束
        /// </summary>
        public ulong end { get; set; }
        /// <summary>
        /// 是否只检查一次
        /// </summary>
        public bool checkonce { get; set; }
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
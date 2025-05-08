using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管线指令
    /// </summary>
    public abstract class Instruct
    {
        /// <summary>
        /// 指令 ID
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// 区间开始
        /// </summary>
        public ulong begin { get; set; }
        /// <summary>
        /// 区间结束
        /// </summary>
        public ulong end { get; set; }
        /// <summary>
        /// 执行条件列表
        /// </summary>
        public List<Condition> conditions { get; set; }
    }
}
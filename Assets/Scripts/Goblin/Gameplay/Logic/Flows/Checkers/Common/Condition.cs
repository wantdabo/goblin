using System;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Common
{
    /// <summary>
    /// 管线指令执行条件
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public abstract class Condition
    {
        /// <summary>
        /// 条件 ID
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// 序列化条件
        /// </summary>
        /// <returns>二进制数据</returns>
        public abstract byte[] Serialize();
    }
}
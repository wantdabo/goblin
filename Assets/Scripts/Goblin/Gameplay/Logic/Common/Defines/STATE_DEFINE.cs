using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// State 定义
    /// </summary>
    public class STATE_DEFINE
    {
        /// <summary>
        /// 无
        /// </summary>
        public const byte NONE = 0;
        /// <summary>
        /// 出生
        /// </summary>
        public const byte BORN = 1;
        /// <summary>
        /// 死亡
        /// </summary>
        public const byte DEATH = 2;
        /// <summary>
        /// 待机
        /// </summary>
        public const byte IDLE = 3;
        /// <summary>
        /// 移动
        /// </summary>
        public const byte MOVE = 4;
        /// <summary>
        /// 跳跃
        /// </summary>
        public const byte JUMP = 5;
        /// <summary>
        /// 下坠
        /// </summary>
        public const byte FALL = 6;
        /// <summary>
        /// 技能
        /// </summary>
        public const byte CASTING = 7;

        /// <summary>
        /// 状态切换规则
        /// </summary>
        public static Dictionary<byte, List<byte>> PASSES { get; private set; } = new()
        {
            { BORN, new List<byte>() { } },
            { DEATH, new List<byte>() { } },
            { IDLE, new List<byte>() { MOVE, FALL, CASTING } },
            { MOVE, new List<byte>() { IDLE, FALL, CASTING } },
            { JUMP, new List<byte>() { FALL, CASTING } },
            { FALL, new List<byte>() { IDLE, CASTING } },
            { CASTING, new List<byte>() { } }
        };
    }
}

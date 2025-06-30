using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// State 定义
    /// </summary>
    public class STATE_DEFINE
    {
        /// <summary>
        /// 出生
        /// </summary>
        public const byte BORN = 0;
        /// <summary>
        /// 死亡
        /// </summary>
        public const byte DEAD = 1;
        /// <summary>
        /// 待机
        /// </summary>
        public const byte IDLE = 2;
        /// <summary>
        /// 移动
        /// </summary>
        public const byte MOVE = 3;
        /// <summary>
        /// 跳跃
        /// </summary>
        public const byte JUMP = 4;
        /// <summary>
        /// 下坠
        /// </summary>
        public const byte FALL = 5;
        /// <summary>
        /// 技能
        /// </summary>
        public const byte CASTING = 6;

        /// <summary>
        /// 状态切换规则
        /// </summary>
        public static Dictionary<byte, List<byte>> PASSES { get; private set; } = new()
        {
            { IDLE, new List<byte>() { MOVE, FALL, CASTING } },
            { MOVE, new List<byte>() { IDLE, FALL, CASTING } },
            { JUMP, new List<byte>() { FALL, CASTING } },
            { FALL, new List<byte>() { IDLE, CASTING } },
            { CASTING, new List<byte>() { } }
        };
    }
}

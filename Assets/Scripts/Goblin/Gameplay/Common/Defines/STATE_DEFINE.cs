namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// State 定义
    /// </summary>
    public class STATE_DEFINE
    {
        /// <summary>
        /// 最大层数
        /// </summary>
        public const byte MAX_LAYER = 2;
        /// <summary>
        /// 第零层
        /// </summary>
        public const byte LAYER_ZERO = 0;
        /// <summary>
        /// 第一层
        /// </summary>
        public const byte LAYER_ONE = 1;
        
        /// <summary>
        /// 空状态
        /// </summary>
        public const uint NULL = uint.MaxValue;
        /// <summary>
        /// 玩家 IDLE 状态
        /// </summary>
        public const uint PLAYER_IDLE = 100001;
        /// <summary>
        /// 玩家 RUN 状态
        /// </summary>
        public const uint PLAYER_RUN = 100002;
        /// <summary>
        /// 玩家 JUMP 状态
        /// </summary>
        public const uint PLAYER_JUMP = 100003;
        /// <summary>
        /// 玩家 ROLL 状态
        /// </summary>
        public const uint PLAYER_ROLL = 100004;
        /// <summary>
        /// 玩家 DEFEND 状态
        /// </summary>
        public const uint PLAYER_DEFEND = 100005;
        /// <summary>
        /// 玩家 HURT 状态
        /// </summary>
        public const uint PLAYER_HURT = 100006;
        /// <summary>
        /// 玩家 DEATH 状态
        /// </summary>
        public const uint PLAYER_DEATH = 100007;
        /// <summary>
        /// 玩家 ATTACK 状态
        /// </summary>
        public const uint PLAYER_ATTACK = 100008;
    }
}

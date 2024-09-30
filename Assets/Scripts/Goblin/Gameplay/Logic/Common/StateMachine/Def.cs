namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    public partial class ParallelMachine
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
    }
    
    public abstract partial class State
    {
        public const uint NULL = uint.MaxValue;
        public const uint PLAYER_IDLE = 100001;
        public const uint PLAYER_RUN = 100002;
    }
}

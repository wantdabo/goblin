using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 座位状态
    /// </summary>
    public struct SeatState : IState
    {
        public StateType type => StateType.Seat;
        public ulong actor { get; set; }
        public uint frame { get; set; }
        public ulong seat { get; set; }
    }
}
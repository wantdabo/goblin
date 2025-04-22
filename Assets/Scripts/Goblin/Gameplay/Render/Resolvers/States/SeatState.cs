using System.Collections.Generic;
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
        /// <summary>
        /// 座位字典, 键为座位 ID, 值为 ActorID
        /// </summary>
        public Dictionary<ulong, ulong> seatdict { get; set; }
    }
}
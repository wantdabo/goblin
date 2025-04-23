using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 座位状态
    /// </summary>
    public class SeatState : State
    {
        public override StateType type => StateType.Seat;
        /// <summary>
        /// 座位字典, 键为座位 ID, 值为 ActorID
        /// </summary>
        public Dictionary<ulong, ulong> seatdict { get; set; }
        
        protected override void OnReset()
        {
            if (null != seatdict)
            {
                seatdict.Clear();
                ObjectCache.Set(seatdict);
            }
        }
    }
}
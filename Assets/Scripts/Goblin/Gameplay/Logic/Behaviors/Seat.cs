using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 座位行为
    /// </summary>
    public class Seat : Behavior<SeatInfo>
    {
        /// <summary>
        /// 根据座位 ID 获取 ActorID
        /// </summary>
        /// <param name="seat">座位 ID</param>
        /// <returns>ActorID</returns>
        public ulong GetActor(ulong seat)
        {
            if (info.sadict.TryGetValue(seat, out var actor))
            {
                return actor;
            }
            return 0;
        }
        
        /// <summary>
        /// 根据 ActorID 获取座位 ID
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <returns>座位 ID</returns>
        public ulong GetSeat(ulong actor)
        {
            if (info.asdict.TryGetValue(actor, out var seat))
            {
                return seat;
            }
            return 0;
        }

        /// <summary>
        /// 坐下
        /// </summary>
        /// <param name="seat">座位 ID</param>
        /// <param name="id">ActorID</param>
        public void Sitdown(ulong seat, ulong id)
        {
            if (info.sadict.ContainsKey(seat)) info.sadict.Remove(seat);
            if (info.asdict.ContainsKey(id)) info.asdict.Remove(id);
            
            info.sadict.Add(seat, id);
            info.asdict.Add(id, seat);
        }

        /// <summary>
        /// 站起
        /// </summary>
        /// <param name="id">ActorID</param>
        public void Standup(ulong id)
        {
            if (false == info.asdict.TryGetValue(id, out var seat)) return;
            
            info.asdict.Remove(id);
            info.sadict.Remove(seat);
        }
    }
}
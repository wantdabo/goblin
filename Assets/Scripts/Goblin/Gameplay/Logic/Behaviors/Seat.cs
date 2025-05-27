using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 座位行为
    /// </summary>
    public class Seat : Behavior<SeatInfo>
    {
        protected override void OnAssemble()
        {
            base.OnAssemble();
            stage.eventor.Listen<ActorDeadEvent>(OnActorDead);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            stage.eventor.UnListen<ActorDeadEvent>(OnActorDead);
        }

        /// <summary>
        /// 根据座位 ID 获取 ActorID
        /// </summary>
        /// <param name="seat">座位 ID</param>
        /// <returns>ActorID</returns>
        public ulong GetActor(ulong seat)
        {
            if (info.sadict.TryGetValue(seat, out var actor)) return actor;
            
            return 0;
        }
        
        /// <summary>
        /// 根据 ActorID 获取座位 ID
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <returns>座位 ID</returns>
        public ulong GetSeat(ulong actor)
        {
            if (info.asdict.TryGetValue(actor, out var seat)) return seat;
            
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
        
        private void OnActorDead(ActorDeadEvent e)
        {
            // 站起
            if (false == info.asdict.TryGetValue(e.actor, out var seat)) return;
            
            info.asdict.Remove(e.actor);
            info.sadict.Remove(seat);
        }
    }
}
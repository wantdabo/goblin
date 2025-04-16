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
            stage.eventor.Listen<ActorDeadEvent>(actor.eventor, OnActorDead);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            stage.eventor.UnListen<ActorDeadEvent>(actor.eventor, OnActorDead);
        }
        
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
        /// 进入座位
        /// </summary>
        /// <param name="seat">座位 ID</param>
        /// <param name="actor">ActorID</param>
        public void Enter(ulong seat, ulong actor)
        {
            if (info.sadict.ContainsKey(seat)) info.sadict.Remove(seat);
            if (info.asdict.ContainsKey(actor)) info.asdict.Remove(actor);
            
            info.sadict.Add(seat, actor);
            info.asdict.Add(actor, seat);
        }

        private void OnActorDead(ActorDeadEvent e)
        {
            if (false == info.asdict.TryGetValue(e.id, out var seat)) return;
            
            info.asdict.Remove(e.id);
            info.sadict.Remove(seat);
        }
    }
}
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Physics.Collisions;
using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class ActorBehavior : Behavior<ActorBehavior.ActorInfo>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Info.actorId = Actor.Theater.NewActorId;
            SendRIL<RILAdd>();
        }

        public void SendRIL<T>(Action<T> action = null) where T : RIL, new()
        {
            T ril = new T();
            ril.actorId = Info.actorId;
            action?.Invoke(ril);
            Actor.Theater.SendRIL(ril);
        }

        #region ActorInfo
        public class ActorInfo : BehaviorInfo
        {
            public int actorId;

            public event Action<Fixed64Vector3> direChanged;
            private Fixed64Vector3 mDire = Fixed64Vector3.forward;
            public Fixed64Vector3 dire
            {
                get { return mDire; }
                set
                {
                    mDire = value;
                    direChanged?.Invoke(dire);
                }
            }

            public event Action<Fixed64Vector3> posChanged;
            private Fixed64Vector3 mPos;
            public Fixed64Vector3 pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                    posChanged?.Invoke(pos);
                }
            }
        }
        #endregion
    }
}

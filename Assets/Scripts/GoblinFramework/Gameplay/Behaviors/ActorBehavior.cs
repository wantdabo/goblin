using BEPUutilities;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.RIL.RILS;
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

            public event Action<Vector3> direChanged;
            private Vector3 mRotation = Vector3.forward;
            public Vector3 rotation
            {
                get { return mRotation; }
                set
                {
                    mRotation = value;
                    direChanged?.Invoke(rotation);
                }
            }

            public event Action<Vector3> posChanged;
            private Vector3 mPos;
            public Vector3 pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                    posChanged?.Invoke(pos);
                }
            }

            public event Action<Vector3> sizeChanged;
            private Vector3 mSize;
            public Vector3 size
            {
                get { return mSize; }
                set
                {
                    mSize = value;
                    sizeChanged?.Invoke(size);
                }
            }
        }
        #endregion
    }
}

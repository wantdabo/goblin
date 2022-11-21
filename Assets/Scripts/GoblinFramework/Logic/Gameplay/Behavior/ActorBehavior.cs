using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;
using FixMath.NET;

namespace GoblinFramework.Logic.Gameplay
{
    public class ActorBehavior : Behavior<ActorBehavior.ActorInfo>
    {
        #region ActorInfo
        public class ActorInfo : BehaviorInfo
        {
            public int actorId;

            public event Action<VoltVector2> posChanged;
            private VoltVector2 mPos;
            public VoltVector2 pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                    posChanged?.Invoke(pos);
                }
            }

            public event Action<Fix64> rotationChanged;
            public Fix64 mRotation = Fix64.Zero;
            public Fix64 rotation
            {
                get { return mRotation; }
                set
                {
                    mRotation = value;
                    rotationChanged?.Invoke(rotation);
                }
            }

            public event Action<VoltVector2> scaleChanged;
            private VoltVector2 mScale;
            public VoltVector2 scale
            {
                get { return mScale; }
                set
                {
                    mScale = value;
                    scaleChanged?.Invoke(scale);
                }
            }
        }
        #endregion
    }
}

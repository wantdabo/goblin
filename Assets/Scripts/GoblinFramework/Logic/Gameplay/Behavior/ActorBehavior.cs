using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay
{
    public class ActorBehavior : Behavior<ActorBehavior.ActorInfo>
    {
        #region ActorInfo
        public class ActorInfo : BehaviorInfo
        {
            public int actorId;

            public event Action<TSVector2> posChanged;
            private TSVector2 mPos;
            public TSVector2 pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                    posChanged?.Invoke(pos);
                }
            }

            public event Action<FP> rotationChanged;
            public FP mRotation = FP.Zero;
            public FP rotation
            {
                get { return mRotation; }
                set
                {
                    mRotation = value;
                    rotationChanged?.Invoke(rotation);
                }
            }

            public event Action<TSVector2> scaleChanged;
            private TSVector2 mScale;
            public TSVector2 scale
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

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

            public event Action<TSVector> posChanged;
            private TSVector mPos;
            public TSVector pos
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

            public event Action<TSVector> scaleChanged;
            private TSVector mScale;
            public TSVector scale
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

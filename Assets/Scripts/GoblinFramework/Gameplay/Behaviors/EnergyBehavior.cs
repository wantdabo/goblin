using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    /// <summary>
    /// 能量转换为运动行为
    /// </summary>
    public class EnergyBehavior : Behavior<EnergyBehavior.EnergyInfo>, IPLateLoop
    {
        public void PLateLoop(int frame, Fix64 detailTime)
        {
            var lossMomentEnergy = Info.momentEnergy;
            Info.momentEnergy -= lossMomentEnergy;

            var motionBehavior = Actor.GetBehavior<MotionBehavior>();
            if (null == motionBehavior) return;
            motionBehavior.Motion(lossMomentEnergy + Info.linearEnergy * detailTime);
        }

        #region EnergyInfo
        public class EnergyInfo : BehaviorInfo
        {
            public Vector3 mLinearEnergy;
            public Vector3 linearEnergy
            {
                get
                {
                    var energy = momentEnergy;
                    
                    var bouncingBehavior = Actor.GetBehavior<BouncingBehavior>();
                    if (null != bouncingBehavior) energy += bouncingBehavior.Info.bouncingEnergy;

                    var gravityBehavior = Actor.GetBehavior<GravityBehavior>();
                    if (null != gravityBehavior) energy += gravityBehavior.Info.gravityEnergy;

                    return energy;
                }
            }

            private Vector3 mMomentEnergy;
            public Vector3 momentEnergy { get { return mMomentEnergy; } set { mMomentEnergy = value;} }
        }
        #endregion
    }
}

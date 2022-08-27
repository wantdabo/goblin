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
            var colliderBehavior = Actor.GetBehavior<ColliderBehavior>();
            if (null != colliderBehavior && colliderBehavior.IsOnGround && Info.linearEnergy.Y <= Fix64.Zero) Info.linearEnergy.Y = Fix64.Zero;

            var motionBehavior = Actor.GetBehavior<MotionBehavior>();
            if (null == motionBehavior) return;
            var lossMomentEnergy = Info.momentEnergy;
            Info.momentEnergy -= lossMomentEnergy;

            Info.linearEnergy -= lossMomentEnergy;
            var lossLinearEnergy = Info.linearEnergy * detailTime;
            Info.linearEnergy -= lossLinearEnergy;
            motionBehavior.Motion(lossMomentEnergy + lossLinearEnergy);
        }

        #region EnergyInfo
        public class EnergyInfo : BehaviorInfo
        {
            public Vector3 linearEnergy;

            private Vector3 mMomentEnergy;
            public Vector3 momentEnergy { get { return mMomentEnergy; } set { mMomentEnergy = value; linearEnergy += mMomentEnergy; } }
        }
        #endregion
    }
}

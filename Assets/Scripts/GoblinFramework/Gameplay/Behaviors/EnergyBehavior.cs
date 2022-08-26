using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
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
        private GravityBehavior gravityBehavior;
        private BouncingBehavior bouncingBehavior;
        private MotionBehavior motionBehavior;
        protected override void OnCreate()
        {
            base.OnCreate();
            gravityBehavior = Actor.GetBehavior<GravityBehavior>();
            bouncingBehavior = Actor.GetBehavior<BouncingBehavior>();
            motionBehavior = Actor.GetBehavior<MotionBehavior>();
        }

        public void PLateLoop(int frame, Fix64 detailTime)
        {
            var force = Vector3.Zero;
            // 重力转换
            if (null != gravityBehavior && gravityBehavior.Info.gravityVelocity != Vector3.Zero) force += gravityBehavior.Info.gravityVelocity * detailTime;
            // 弹跳力转换
            if (null != bouncingBehavior && bouncingBehavior.Info.bouncingVelocity != Vector3.Zero) force += bouncingBehavior.Info.bouncingVelocity;

            Info.velocity = force;
            motionBehavior?.AddForce(force);
        }

        #region EnergyInfo
        public class EnergyInfo : BehaviorInfo
        {
            public Vector3 velocity = Vector3.Zero;
        }
        #endregion
    }
}

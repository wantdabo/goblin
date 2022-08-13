using FixMath.NET;
using BEPUutilities;

namespace BEPUik
{
    /// <summary>
    /// Constrains an individual bone in an attempt to reach some position and orientation goal.
    /// </summary>
    public class StateControl : Control
    {
        /// <summary>
        /// Gets or sets the controlled bone.
        /// </summary>
        public override Bone TargetBone
        {
            get { return LinearMotor.TargetBone; }
            set
            {
                LinearMotor.TargetBone = value;
                AngularMotor.TargetBone = value;
                if (value != null)
                    AngularMotor.TargetOrientation = value.Orientation;
            }
        }

        /// <summary>
        /// Gets the linear motor used by the control.
        /// </summary>
        public SingleBoneLinearMotor LinearMotor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the angular motor used by the control.
        /// </summary>
        public SingleBoneAngularMotor AngularMotor
        {
            get;
            private set;
        }

        public StateControl()
        {
            LinearMotor = new SingleBoneLinearMotor();
            AngularMotor = new SingleBoneAngularMotor();
            LinearMotor.Rigidity = F64.C1;
            AngularMotor.Rigidity = F64.C1;
        }

        protected internal override void Preupdate(Fix64 dt, Fix64 updateRate)
        {
            LinearMotor.Preupdate(dt, updateRate);
            AngularMotor.Preupdate(dt, updateRate);
        }


        protected internal override void UpdateJacobiansAndVelocityBias()
        {
            LinearMotor.UpdateJacobiansAndVelocityBias();
            AngularMotor.UpdateJacobiansAndVelocityBias();
        }

        protected internal override void ComputeEffectiveMass()
        {
            LinearMotor.ComputeEffectiveMass();
            AngularMotor.ComputeEffectiveMass();
        }

        protected internal override void WarmStart()
        {
            LinearMotor.WarmStart();
            AngularMotor.WarmStart();
        }

        protected internal override void SolveVelocityIteration()
        {
            LinearMotor.SolveVelocityIteration();
            AngularMotor.SolveVelocityIteration();
        }

        protected internal override void ClearAccumulatedImpulses()
        {
            LinearMotor.ClearAccumulatedImpulses();
            AngularMotor.ClearAccumulatedImpulses();
        }

        public override Fix64 MaximumForce
        {
            get { return LinearMotor.MaximumForce; }
            set
            {
                LinearMotor.MaximumForce = value;
                AngularMotor.MaximumForce = value;
            }
        }
    }
}
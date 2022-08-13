using FixMath.NET;

namespace BEPUik
{
    /// <summary>
    /// Constrains an individual bone in an attempt to reach some goal.
    /// Controls act as groups of single bone constraints. They are used
    /// by the solver to determine the active set of body constraints.
    /// </summary>
    public abstract class Control
    {
        /// <summary>
        /// Gets or sets the controlled bone.
        /// </summary>
        public abstract Bone TargetBone { get; set; }

        protected internal abstract void Preupdate(Fix64 dt, Fix64 updateRate);

        protected internal abstract void UpdateJacobiansAndVelocityBias();

        protected internal abstract void ComputeEffectiveMass();

        protected internal abstract void WarmStart();

        protected internal abstract void SolveVelocityIteration();

        protected internal abstract void ClearAccumulatedImpulses();

        public abstract Fix64 MaximumForce { get; set; }
    }
}
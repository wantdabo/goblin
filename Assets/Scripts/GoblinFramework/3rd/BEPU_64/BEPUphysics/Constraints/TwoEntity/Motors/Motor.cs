using FixMath.NET;

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Superclass of constraints which do work and change the velocity of connected entities, but have no specific position target.
    /// </summary>
    public abstract class Motor : TwoEntityConstraint
    {
        protected Fix64 maxForceDt = Fix64.MaxValue;
        protected Fix64 maxForceDtSquared = Fix64.MaxValue;

        /// <summary>
        /// Softness divided by the timestep to maintain timestep independence.
        /// </summary>
        internal Fix64 usedSoftness;

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        protected void ComputeMaxForces(Fix64 maxForce, Fix64 dt)
        {
            //Determine maximum force
            if (maxForce < Fix64.MaxValue)
            {
                maxForceDt = maxForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = Fix64.MaxValue;
                maxForceDtSquared = Fix64.MaxValue;
            }
        }
    }
}
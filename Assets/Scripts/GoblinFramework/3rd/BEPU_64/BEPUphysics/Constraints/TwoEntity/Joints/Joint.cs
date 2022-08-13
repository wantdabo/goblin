using BEPUutilities;
using FixMath.NET;
using System;

namespace BEPUphysics.Constraints.TwoEntity.Joints
{
    /// <summary>
    /// Superclass of position-based constraints.
    /// </summary>
    public abstract class Joint : TwoEntityConstraint, ISpringSettings
    {
        /// <summary>
        /// Maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected Fix64 maxCorrectiveVelocity = Fix64.MaxValue;

        /// <summary>
        /// Squared maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected Fix64 maxCorrectiveVelocitySquared = Fix64.MaxValue;

        protected Fix64 softness;

        /// <summary>
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        protected SpringSettings springSettings = new SpringSettings();

        /// <summary>
        /// Gets or sets the maximum extra velocity that the constraint will apply in an effort to correct any constraint error.
        /// </summary>
        public Fix64 MaxCorrectiveVelocity
        {
            get { return maxCorrectiveVelocity; }
            set
            {
                maxCorrectiveVelocity = MathHelper.Max(F64.C0, value);
                if (maxCorrectiveVelocity >= Fix64.MaxValue)
                {
                    maxCorrectiveVelocitySquared = Fix64.MaxValue;
                }
                else
                {
                    maxCorrectiveVelocitySquared = maxCorrectiveVelocity * maxCorrectiveVelocity;
                }
            }
        }

        #region ISpringSettings Members

        /// <summary>
        /// Gets the spring settings used by the constraint.
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        public SpringSettings SpringSettings
        {
            get { return springSettings; }
        }

        #endregion
    }
}
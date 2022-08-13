using System;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.Constraints
{
    /// <summary>
    /// Contains the error reduction factor and softness of a constraint.
    /// These can be used to make the same behaviors as the stiffness and damping constants,
    /// but may provide a more intuitive representation for rigid constraints.
    /// </summary>
    public class SpringAdvancedSettings
    {
        internal Fix64 errorReductionFactor = (Fix64).1m;

        internal Fix64 softness = (Fix64).00001m;

        internal bool useAdvancedSettings;

        /// <summary>
        /// Gets or sets the error reduction parameter of the spring.
        /// </summary>
        public Fix64 ErrorReductionFactor
        {
            get { return errorReductionFactor; }
            set { errorReductionFactor = MathHelper.Clamp(value, F64.C0, F64.C1); }
        }

        /// <summary>
        /// Gets or sets the softness of the joint.  Higher values allow the constraint to be violated more.
        /// </summary>
        public Fix64 Softness
        {
            get { return softness; }
            set { softness = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Gets or sets whether or not to use the advanced settings.
        /// If this is set to true, the errorReductionFactor and softness will be used instead
        /// of the stiffness constant and damping constant.
        /// </summary>
        public bool UseAdvancedSettings
        {
            get { return useAdvancedSettings; }
            set { useAdvancedSettings = value; }
        }
    }


    /// <summary>
    /// Specifies the way in which a constraint's spring component behaves.
    /// </summary>
    public class SpringSettings
    {
        private readonly SpringAdvancedSettings advanced = new SpringAdvancedSettings();

        internal Fix64 damping = F64.C90000;
        internal Fix64 stiffness = F64.C600000;

        /// <summary>
        /// Gets an object containing the solver's direct view of the spring behavior.
        /// </summary>
        public SpringAdvancedSettings Advanced
        {
            get { return advanced; }
        }

        /// <summary>
        /// Gets or sets the damping coefficient of this spring.  Higher values reduce oscillation more.
        /// </summary>
        public Fix64 Damping
        {
            get { return damping; }
            set { damping = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Gets or sets the stiffness coefficient of this spring.  Higher values make the spring stiffer.
        /// </summary>
        public Fix64 Stiffness
        {
            get { return stiffness; }
            set { stiffness = MathHelper.Max(F64.C0, value); }
        }

        /// <summary>
        /// Computes the error reduction parameter and softness of a constraint based on its constants.
        /// Automatically called by constraint presteps to compute their per-frame values.
        /// </summary>
        /// <param name="dt">Simulation timestep.</param>
        /// <param name="updateRate">Inverse simulation timestep.</param>
        /// <param name="errorReduction">Error reduction factor to use this frame.</param>
        /// <param name="softness">Adjusted softness of the constraint for this frame.</param>
        public void ComputeErrorReductionAndSoftness(Fix64 dt, Fix64 updateRate, out Fix64 errorReduction, out Fix64 softness)
        {
            if (advanced.useAdvancedSettings)
            {
                errorReduction = advanced.errorReductionFactor * updateRate;
                softness = advanced.softness * updateRate;
            }
            else
            {
                if (stiffness == F64.C0 && damping == F64.C0)
                    throw new InvalidOperationException("Constraints cannot have both 0 stiffness and 0 damping.");
                Fix64 multiplier = F64.C1 / (dt * stiffness + damping);
                errorReduction = stiffness * multiplier;
                softness = updateRate * multiplier;
            }
        }
    }
}
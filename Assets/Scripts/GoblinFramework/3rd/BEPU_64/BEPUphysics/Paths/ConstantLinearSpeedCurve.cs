

using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.Paths
{
    /// <summary>
    /// Wrapper around a 3d position curve that specifies a specific velocity at which to travel.
    /// </summary>
    public class ConstantLinearSpeedCurve : ConstantSpeedCurve<Vector3>
    {
        /// <summary>
        /// Constructs a new constant speed curve.
        /// </summary>
        /// <param name="speed">Speed to maintain while traveling around a curve.</param>
        /// <param name="curve">Curve to wrap.</param>
        public ConstantLinearSpeedCurve(Fix64 speed, Curve<Vector3> curve)
            : base(speed, curve)
        {
        }

        /// <summary>
        /// Constructs a new constant speed curve.
        /// </summary>
        /// <param name="speed">Speed to maintain while traveling around a curve.</param>
        /// <param name="curve">Curve to wrap.</param>
        /// <param name="sampleCount">Number of samples to use when constructing the wrapper curve.
        /// More samples increases the accuracy of the speed requirement at the cost of performance.</param>
        public ConstantLinearSpeedCurve(Fix64 speed, Curve<Vector3> curve, int sampleCount)
            : base(speed, curve, sampleCount)
        {
        }

        protected override Fix64 GetDistance(Vector3 start, Vector3 end)
        {
            Fix64 distance;
            Vector3.Distance(ref start, ref end, out distance);
            return distance;
        }
    }
}
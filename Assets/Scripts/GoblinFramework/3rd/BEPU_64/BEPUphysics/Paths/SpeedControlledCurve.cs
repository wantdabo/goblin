using System;
using System.Collections.Generic;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.Paths
{
    internal struct SpeedControlledCurveSample
    {
        public Fix64 Wrapped;
        public Fix64 SpeedControlled;
    }

    /// <summary>
    /// Wrapper that controls the speed at which a curve is traversed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Even if a curve is evaluated at linearly increasing positions,
    /// the distance between consecutive values can be different.  This
    /// has the effect of a curve-following object having variable velocity.
    /// </para>
    /// <para>
    /// To counteract the variable velocity, this wrapper samples the curve
    /// and produces a reparameterized, distance-based curve.  Changing the
    /// evaluated curve position will linearly change the value.
    /// </para>
    /// </remarks>
    public abstract class SpeedControlledCurve<TValue> : Path<TValue>
    {
        private readonly List<SpeedControlledCurveSample> samples = new List<SpeedControlledCurveSample>(); //  X is wrapped view, Y is associated curve view

        private Curve<TValue> curve;
        private int samplesPerInterval;


        /// <summary>
        /// Constructs a new speed controlled curve.
        /// </summary>
        protected SpeedControlledCurve()
        {
        }

        /// <summary>
        /// Constructs a new speed-controlled curve.
        /// </summary>
        /// <param name="curve">Curve to wrap.</param>
        protected SpeedControlledCurve(Curve<TValue> curve)
        {
            samplesPerInterval = 10;
            this.curve = curve;
        }

        /// <summary>
        /// Constructs a new speed-controlled curve.
        /// </summary>
        /// <param name="curve">Curve to wrap.</param>
        /// <param name="samplesPerInterval">Number of samples to use when constructing the wrapper curve.
        /// More samples increases the accuracy of the speed requirement at the cost of performance.</param>
        protected SpeedControlledCurve(Curve<TValue> curve, int samplesPerInterval)
        {
            this.curve = curve;
            this.samplesPerInterval = samplesPerInterval;
        }

        /// <summary>
        /// Gets or sets the curve wrapped by this instance.
        /// </summary>
        public Curve<TValue> Curve
        {
            get { return curve; }
            set
            {
                curve = value;
                if (Curve != null)
                    ResampleCurve();
            }
        }

        /// <summary>
        /// Defines how the curve is sampled when the evaluation time exceeds the final control point.
        /// </summary>
        public CurveEndpointBehavior PostLoop { get; set; }

        /// <summary>
        /// Defines how the curve is sampled when the evaluation time exceeds the beginning control point.
        /// </summary>
        public CurveEndpointBehavior PreLoop { get; set; }

        /// <summary>
        /// Gets or sets the number of samples to use per interval in the curve.
        /// </summary>
        public int SamplesPerInterval
        {
            get { return samplesPerInterval; }
            set
            {
                samplesPerInterval = value;
                if (Curve != null)
                    ResampleCurve();
            }
        }

        /// <summary>
        /// Gets the desired speed at a given time.
        /// </summary>
        /// <param name="time">Time to check for speed.</param>
        /// <returns>Speed at the given time.</returns>
        public abstract Fix64 GetSpeedAtCurveTime(Fix64 time);

        /// <summary>
        /// Gets the time at which the internal curve would be evaluated at the given time.
        /// </summary>
        /// <param name="time">Time to evaluate the speed-controlled curve.</param>
        /// <returns>Time at which the internal curve would be evaluated.</returns>
        public Fix64 GetInnerTime(Fix64 time)
        {
            if (Curve == null)
                throw new InvalidOperationException("SpeedControlledCurve's internal curve is null; ensure that its curve property is set prior to evaluation.");
            Fix64 firstTime, lastTime;
            GetPathBoundsInformation(out firstTime, out lastTime);
            time = Curve<TValue>.ModifyTime(time, firstTime, lastTime, Curve.PreLoop, Curve.PostLoop);

            int indexMin = 0;
            int indexMax = samples.Count;
            if (indexMax == 1)
            {
                //1-length curve; asking the system to evaluate
                //this will be a waste of time AND
                //crash since +1 will be outside scope
                return samples[0].SpeedControlled;
            }

            if (indexMax == 0)
            {
                return F64.C0;
            }
            //If time < controlpoints.mintime, should be... 0 or -1?
            while (indexMax - indexMin > 1) //if time belongs to min
            {
                int midIndex = (indexMin + indexMax) / 2;
                if (time > samples[midIndex].Wrapped)
                {
                    indexMin = midIndex;
                }
                else
                {
                    indexMax = midIndex;
                }
            }


            Fix64 curveTime = (time - samples[indexMin].Wrapped) / (samples[indexMin + 1].Wrapped - samples[indexMin].Wrapped);
            return (F64.C1 - curveTime) * samples[indexMin].SpeedControlled + (curveTime) * samples[indexMin + 1].SpeedControlled;
        }

        /// <summary>
        /// Computes the value of the curve at a given time.
        /// </summary>
        /// <param name="time">Time to evaluate the curve at.</param>
        /// <param name="value">Value of the curve at the given time.</param>
        /// <param name="innerTime">Time at which the internal curve was evaluated to get the value.</param>
        public void Evaluate(Fix64 time, out TValue value, out Fix64 innerTime)
        {
            Curve.Evaluate(innerTime = GetInnerTime(time), out value);
        }

        /// <summary>
        /// Computes the value of the curve at a given time.
        /// </summary>
        /// <param name="time">Time to evaluate the curve at.</param>
        /// <param name="value">Value of the curve at the given time.</param>
        public override void Evaluate(Fix64 time, out TValue value)
        {
            Curve.Evaluate(GetInnerTime(time), out value);
        }

        /// <summary>
        /// Gets the starting and ending times of the path.
        /// </summary>
        /// <param name="startingTime">Beginning time of the path.</param>
        /// <param name="endingTime">Ending time of the path.</param>
        public override void GetPathBoundsInformation(out Fix64 startingTime, out Fix64 endingTime)
        {
            if (samples.Count > 0)
            {
                startingTime = F64.C0;
                endingTime = samples[samples.Count - 1].Wrapped;
            }
            else
            {
                startingTime = F64.C0;
                endingTime = F64.C0;
            }
        }

        /// <summary>
        /// Forces a recalculation of curve samples.
        /// This needs to be called if the wrapped curve
        /// is changed.
        /// </summary>
        public void ResampleCurve()
        {
            //TODO: Call this from curve if add/remove/timechange/valuechange happens
            //Could hide it then.
            samples.Clear();
            Fix64 firstTime, lastTime;
            int minIndex, maxIndex;
            curve.GetCurveBoundsInformation(out firstTime, out lastTime, out minIndex, out maxIndex);

            //Curve isn't valid.
            if (minIndex < 0 || maxIndex < 0)
                return;

            Fix64 timeElapsed = F64.C0;
            //TODO: useless calculation due to this
            TValue currentValue = Curve.ControlPoints[minIndex].Value;
            TValue previousValue = currentValue;

            Fix64 inverseSampleCount = 1 / (SamplesPerInterval + 1);

            Fix64 speed = GetSpeedAtCurveTime(Curve.ControlPoints[minIndex].Time);
            Fix64 previousSpeed = speed;
            for (int i = minIndex; i < maxIndex; i++)
            {
                previousValue = currentValue;
                currentValue = Curve.ControlPoints[i].Value;

                if (speed != F64.C0)
                    timeElapsed += GetDistance(previousValue, currentValue) / speed;
                previousSpeed = speed;
                speed = GetSpeedAtCurveTime(Curve.ControlPoints[i].Time);

                samples.Add(new SpeedControlledCurveSample { Wrapped = timeElapsed, SpeedControlled = Curve.ControlPoints[i].Time });

                var curveTime = Curve.ControlPoints[i].Time;
                var intervalLength = Curve.ControlPoints[i + 1].Time - curveTime;
                var curveTimePerSample = intervalLength / (SamplesPerInterval + 1);
                for (int j = 1; j <= SamplesPerInterval; j++)
                {
                    previousValue = currentValue;
                    Curve.Evaluate(i, j * inverseSampleCount, out currentValue);

                    curveTime += curveTimePerSample;
                    if (speed != F64.C0)
                        timeElapsed += GetDistance(previousValue, currentValue) / speed;

                    previousSpeed = speed;
                    speed = GetSpeedAtCurveTime(curveTime);

                    samples.Add(new SpeedControlledCurveSample { Wrapped = timeElapsed, SpeedControlled = curveTime });
                }
            }
            timeElapsed += GetDistance(previousValue, currentValue) / previousSpeed;
            samples.Add(new SpeedControlledCurveSample { Wrapped = timeElapsed, SpeedControlled = Curve.ControlPoints[maxIndex].Time });
        }

        /// <summary>
        /// Computes the distance between the two values.
        /// </summary>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Ending value.</param>
        /// <returns>Distance between the values.</returns>
        protected abstract Fix64 GetDistance(TValue start, TValue end);
    }
}
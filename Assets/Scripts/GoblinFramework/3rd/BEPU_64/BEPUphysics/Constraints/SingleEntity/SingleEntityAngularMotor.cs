using System;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Constraint which attempts to restrict the relative angular velocity of two entities to some value.
    /// Can use a target relative orientation to apply additional force.
    /// </summary>
    public class SingleEntityAngularMotor : SingleEntityConstraint, I3DImpulseConstraintWithError
    {
        private readonly JointBasis3D basis = new JointBasis3D();

        private readonly MotorSettingsOrientation settings;
        private Vector3 accumulatedImpulse;


        private Fix64 angle;
        private Vector3 axis;

        private Vector3 biasVelocity;
        private Matrix3x3 effectiveMassMatrix;

        private Fix64 maxForceDt;
        private Fix64 maxForceDtSquared;
        private Fix64 usedSoftness;

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the angular velocity or orientation to a goal.
        /// </summary>
        /// <param name="entity">Affected entity.</param>
        public SingleEntityAngularMotor(Entity entity)
        {
            Entity = entity;

            settings = new MotorSettingsOrientation(this) {servo = {goal = base.entity.orientation}};
            //Since no target relative orientation was specified, just use the current relative orientation.  Prevents any nasty start-of-sim 'snapping.'

            //mySettings.myServo.springSettings.stiffnessConstant *= .5f;
        }

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the angular velocity or orientation to a goal.
        /// This constructor will make the angular motor start with isActive set to false.
        /// </summary>
        public SingleEntityAngularMotor()
        {
            settings = new MotorSettingsOrientation(this);
            IsActive = false;
        }

        /// <summary>
        /// Gets the basis attached to the entity.
        /// The target velocity/orientation of this motor is transformed by the basis.
        /// </summary>
        public JointBasis3D Basis
        {
            get { return basis; }
        }

        /// <summary>
        /// Gets the motor's velocity and servo settings.
        /// </summary>
        public MotorSettingsOrientation Settings
        {
            get { return settings; }
        }

        #region I3DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity with respect to the constraint.
        /// For single entity constraints, this is pretty straightforward.  It is taken directly from the 
        /// entity.
        /// </summary>
        public Vector3 RelativeVelocity
        {
            get { return -Entity.AngularVelocity; }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public Vector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// If the motor is in velocity only mode, error is zero.
        /// </summary>
        public Vector3 Error
        {
            get { return axis * angle; }
        }

        #endregion

        /// <summary>
        /// Applies the corrective impulses required by the constraint.
        /// </summary>
        public override Fix64 SolveIteration()
        {
#if !WINDOWS
            Vector3 lambda = new Vector3();
#else
            Vector3 lambda;
#endif
            Vector3 aVel = entity.angularVelocity;
            lambda.X = -aVel.X + biasVelocity.X - usedSoftness * accumulatedImpulse.X;
            lambda.Y = -aVel.Y + biasVelocity.Y - usedSoftness * accumulatedImpulse.Y;
            lambda.Z = -aVel.Z + biasVelocity.Z - usedSoftness * accumulatedImpulse.Z;

            Matrix3x3.Transform(ref lambda, ref effectiveMassMatrix, out lambda);

            Vector3 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse.X += lambda.X;
            accumulatedImpulse.Y += lambda.Y;
            accumulatedImpulse.Z += lambda.Z;
            Fix64 sumLengthSquared = accumulatedImpulse.LengthSquared();

            if (sumLengthSquared > maxForceDtSquared)
            {
                //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                Fix64 multiplier = maxForceDt / Fix64.Sqrt(sumLengthSquared);
                accumulatedImpulse.X *= multiplier;
                accumulatedImpulse.Y *= multiplier;
                accumulatedImpulse.Z *= multiplier;

                //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                lambda.X = accumulatedImpulse.X - previousAccumulatedImpulse.X;
                lambda.Y = accumulatedImpulse.Y - previousAccumulatedImpulse.Y;
                lambda.Z = accumulatedImpulse.Z - previousAccumulatedImpulse.Z;
            }


            entity.ApplyAngularImpulse(ref lambda);


            return Fix64.Abs(lambda.X) + Fix64.Abs(lambda.Y) + Fix64.Abs(lambda.Z);
        }

        /// <summary>
        /// Initializes the constraint for the current frame.
        /// </summary>
        /// <param name="dt">Time between frames.</param>
        public override void Update(Fix64 dt)
        {
            basis.rotationMatrix = entity.orientationMatrix;
            basis.ComputeWorldSpaceAxes();

            Fix64 updateRate = F64.C1 / dt;
            if (settings.mode == MotorMode.Servomechanism) //Only need to do the bulk of this work if it's a servo.
            {
                Quaternion currentRelativeOrientation;
                var worldTransform = basis.WorldTransform;
                Quaternion.CreateFromRotationMatrix(ref worldTransform, out currentRelativeOrientation);


                //Compute the relative orientation R' between R and the target relative orientation.
                Quaternion errorOrientation;
                Quaternion.Conjugate(ref currentRelativeOrientation, out errorOrientation);
                Quaternion.Multiply(ref settings.servo.goal, ref errorOrientation, out errorOrientation);


                Fix64 errorReduction;
                settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, updateRate, out errorReduction, out usedSoftness);

                //Turn this into an axis-angle representation.
                Quaternion.GetAxisAngleFromQuaternion(ref errorOrientation, out axis, out angle);

                //Scale the axis by the desired velocity if the angle is sufficiently large (epsilon).
                if (angle > Toolbox.BigEpsilon)
                {
                    Fix64 velocity = MathHelper.Min(settings.servo.baseCorrectiveSpeed, angle * updateRate) + angle * errorReduction;

                    biasVelocity.X = axis.X * velocity;
                    biasVelocity.Y = axis.Y * velocity;
                    biasVelocity.Z = axis.Z * velocity;


                    //Ensure that the corrective velocity doesn't exceed the max.
                    Fix64 length = biasVelocity.LengthSquared();
                    if (length > settings.servo.maxCorrectiveVelocitySquared)
                    {
                        Fix64 multiplier = settings.servo.maxCorrectiveVelocity / Fix64.Sqrt(length);
                        biasVelocity.X *= multiplier;
                        biasVelocity.Y *= multiplier;
                        biasVelocity.Z *= multiplier;
                    }
                }
                else
                {
                    //Wouldn't want an old frame's bias velocity to sneak in.
                    biasVelocity = new Vector3();
                }
            }
            else
            {
                usedSoftness = settings.velocityMotor.softness * updateRate;
                angle = F64.C0; //Zero out the error;
                Matrix3x3 transform = basis.WorldTransform;
                Matrix3x3.Transform(ref settings.velocityMotor.goalVelocity, ref transform, out biasVelocity);
            }

            //Compute effective mass
            effectiveMassMatrix = entity.inertiaTensorInverse;
            effectiveMassMatrix.M11 += usedSoftness;
            effectiveMassMatrix.M22 += usedSoftness;
            effectiveMassMatrix.M33 += usedSoftness;
            Matrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);

            //Update the maximum force
            ComputeMaxForces(settings.maximumForce, dt);


            
        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Apply accumulated impulse
            entity.ApplyAngularImpulse(ref accumulatedImpulse);
        }

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        private void ComputeMaxForces(Fix64 maxForce, Fix64 dt)
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
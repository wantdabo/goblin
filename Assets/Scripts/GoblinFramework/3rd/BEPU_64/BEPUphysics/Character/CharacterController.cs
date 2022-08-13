using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems;
using BEPUutilities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Materials;
using BEPUphysics.PositionUpdating;
using System.Diagnostics;
using System.Threading;
using FixMath.NET;

namespace BEPUphysics.Character
{
    /// <summary>
    /// Gives a physical object FPS-like control, including stepping and jumping.
    /// </summary>
    public class CharacterController : Updateable, IBeforeSolverUpdateable
    {
        /// <summary>
        /// Gets the physical body of the character.  Do not use this reference to modify the character's height and radius.  Instead, use the BodyRadius property and the StanceManager's StandingHeight and CrouchingHeight properties.
        /// </summary>
        public Cylinder Body { get; private set; }

        /// <summary>
        /// Gets the contact categorizer used by the character to determine how contacts affect the character's movement.
        /// </summary>
        public CharacterContactCategorizer ContactCategorizer { get; private set; }

        /// <summary>
        /// Gets the manager responsible for finding places for the character to step up and down to.
        /// </summary>
        public StepManager StepManager { get; private set; }

        /// <summary>
        /// Gets the manager responsible for crouching, standing, and the verification involved in changing states.
        /// </summary>
        public StanceManager StanceManager { get; private set; }

        /// <summary>
        /// Gets the support system which other systems use to perform local ray casts and contact queries.
        /// </summary>
        public QueryManager QueryManager { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to handle horizontal motion.  This includes acceleration due to player input and deceleration when the relative velocity
        /// between the support and the character exceeds specified maximums.
        /// </summary>
        public HorizontalMotionConstraint HorizontalMotionConstraint { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to stay glued to surfaces it stands on.
        /// </summary>
        public VerticalMotionConstraint VerticalMotionConstraint { get; private set; }

        /// <summary>
        /// Gets or sets the pair locker used by the character controller to avoid interfering with the behavior of other characters.
        /// </summary>
        private CharacterPairLocker PairLocker { get; set; }

        /// <summary>
        /// Gets or sets the down direction of the character, defining its orientation.
        /// </summary>
        public Vector3 Down
        {
            get
            {
                return Body.OrientationMatrix.Down;
            }
            set
            {
                //Update the character's orientation to something compatible with the new direction.
                Quaternion orientation;
                Fix64 lengthSquared = value.LengthSquared();
                if (lengthSquared < Toolbox.Epsilon)
                    value = Body.OrientationMatrix.Down; //Silently fail. Assuming here that a dynamic process is setting this property; don't need to make a stink about it.
                else
                    Vector3.Divide(ref value, Fix64.Sqrt(lengthSquared), out value);
                Quaternion.GetQuaternionBetweenNormalizedVectors(ref Toolbox.DownVector, ref value, out orientation);
                Body.Orientation = orientation;
            }
        }

        Vector3 viewDirection = new Vector3(F64.C0, F64.C0, -1);

        /// <summary>
        /// Gets or sets the view direction associated with the character.
        /// Also sets the horizontal view direction internally based on the current down vector.
        /// This is used to interpret the movement directions.
        /// </summary>
        public Vector3 ViewDirection
        {
            get
            {
                return viewDirection;
            }
            set
            {
                Fix64 lengthSquared = value.LengthSquared();
                if (lengthSquared > F64.C1em7)
                {
                    Vector3.Divide(ref value, Fix64.Sqrt(lengthSquared), out viewDirection);
                }
                else
                {
                    value = Vector3.Cross(Down, Toolbox.UpVector);
                    lengthSquared = value.LengthSquared();
                    if (lengthSquared > F64.C1em7)
                    {
                        Vector3.Divide(ref value, Fix64.Sqrt(lengthSquared), out viewDirection);
                    }
                    else
                    {
                        value = Vector3.Cross(Down, Toolbox.ForwardVector);
                        Vector3.Normalize(ref value, out viewDirection);
                    }
                }
            }
        }

        private Fix64 jumpSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character leaves the ground when it jumps.
        /// </summary>
        public Fix64 JumpSpeed
        {
            get
            {
                return jumpSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                jumpSpeed = value;
            }
        }
        Fix64 slidingJumpSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character leaves the ground when it jumps without traction.
        /// </summary>
        public Fix64 SlidingJumpSpeed
        {
            get
            {
                return slidingJumpSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                slidingJumpSpeed = value;
            }
        }
        Fix64 jumpForceFactor = F64.C1;
        /// <summary>
        /// Gets or sets the amount of force to apply to supporting dynamic entities as a fraction of the force used to reach the jump speed.
        /// </summary>
        public Fix64 JumpForceFactor
        {
            get
            {
                return jumpForceFactor;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                jumpForceFactor = value;
            }
        }

        Fix64 standingSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character will try to move while standing with a support that provides traction.
        /// Relative velocities with a greater magnitude will be decelerated.
        /// </summary>
        public Fix64 StandingSpeed
        {
            get
            {
                return standingSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                standingSpeed = value;
            }
        }
        Fix64 crouchingSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character will try to move while crouching with a support that provides traction.
        /// Relative velocities with a greater magnitude will be decelerated.
        /// </summary>
        public Fix64 CrouchingSpeed
        {
            get
            {
                return crouchingSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                crouchingSpeed = value;
            }
        }
        Fix64 proneSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character will try to move while prone with a support that provides traction.
        /// Relative velocities with a greater magnitude will be decelerated.
        /// </summary>
        public Fix64 ProneSpeed
        {
            get
            {
                return proneSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                proneSpeed = value;
            }
        }
        Fix64 tractionForce;
        /// <summary>
        /// Gets or sets the maximum force that the character can apply while on a support which provides traction.
        /// </summary>
        public Fix64 TractionForce
        {
            get
            {
                return tractionForce;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                tractionForce = value;
            }
        }

        Fix64 slidingSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character will try to move while on a support that does not provide traction.
        /// Relative velocities with a greater magnitude will be decelerated.
        /// </summary>
        public Fix64 SlidingSpeed
        {
            get
            {
                return slidingSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                slidingSpeed = value;
            }
        }
        Fix64 slidingForce;
        /// <summary>
        /// Gets or sets the maximum force that the character can apply while on a support which does not provide traction.
        /// </summary>
        public Fix64 SlidingForce
        {
            get
            {
                return slidingForce;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                slidingForce = value;
            }
        }

        Fix64 airSpeed;
        /// <summary>
        /// Gets or sets the speed at which the character will try to move with no support.
        /// The character will not be decelerated while airborne.
        /// </summary>
        public Fix64 AirSpeed
        {
            get
            {
                return airSpeed;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                airSpeed = value;
            }
        }
        Fix64 airForce;
        /// <summary>
        /// Gets or sets the maximum force that the character can apply with no support.
        /// </summary>
        public Fix64 AirForce
        {
            get
            {
                return airForce;
            }
            set
            {
                if (value < F64.C0)
                    throw new ArgumentException("Value must be nonnegative.");
                airForce = value;
            }
        }

        private Fix64 speedScale = F64.C1;
        /// <summary>
        /// Gets or sets a scaling factor to apply to the maximum speed of the character.
        /// This is useful when a character does not have 0 or MaximumSpeed target speed, but rather
        /// intermediate values. A common use case is analog controller sticks.
        /// </summary>
        public Fix64 SpeedScale
        {
            get { return speedScale; }
            set { speedScale = value; }
        }


        /// <summary>
        /// Gets or sets the radius of the body cylinder.  To change the height, use the StanceManager.StandingHeight and StanceManager.CrouchingHeight.
        /// </summary>
        public Fix64 BodyRadius
        {
            get { return Body.CollisionInformation.Shape.Radius; }
            set
            {
                if (value <= F64.C0)
                    throw new ArgumentException("Radius must be positive.");
                Body.CollisionInformation.Shape.Radius = value;
                //Tell the query manager to update its representation.
                StanceManager.UpdateQueryShapes();
            }
        }

        /// <summary>
        /// Gets or sets the collision margin of the body cylinder. Also updates the StanceManager's query shapes to match.
        /// </summary>
        public Fix64 CollisionMargin
        {
            get { return Body.CollisionInformation.Shape.CollisionMargin; }
            set
            {
                if (value <= F64.C0)
                    throw new ArgumentException("Radius must be positive.");
                Body.CollisionInformation.Shape.CollisionMargin = value;
                //Tell the query manager to update its representation.
                StanceManager.UpdateQueryShapes();
            }
        }

        /// <summary>
        /// Gets the support finder used by the character.
        /// The support finder analyzes the character's contacts to see if any of them provide support and/or traction.
        /// </summary>
        public SupportFinder SupportFinder { get; private set; }


		/// <summary>
		/// Constructs a new character controller.
		/// </summary>
		/// <param name="position">Initial position of the character.</param>
		/// <param name="height">Height of the character body while standing.</param>
		/// <param name="crouchingHeight">Height of the character body while crouching.</param>
		/// <param name="proneHeight">Height of the character body while prone.</param>
		/// <param name="radius">Radius of the character body.</param>
		/// <param name="margin">Radius of 'rounding' applied to the cylindrical body. Higher values make the cylinder's edges more rounded.
		/// The margin is contained within the cylinder's height and radius, so it must not exceed the radius or height of the cylinder.
		/// To change the collision margin later, use the CharacterController.CollisionMargin property.</param>
		/// <param name="mass">Mass of the character body.</param>
		/// <param name="maximumTractionSlope">Steepest slope, in radians, that the character can maintain traction on.</param>
		/// <param name="maximumSupportSlope">Steepest slope, in radians, that the character can consider a support.</param>
		/// <param name="standingSpeed">Speed at which the character will try to move while crouching with a support that provides traction.
		/// Relative velocities with a greater magnitude will be decelerated.</param>
		/// <param name="crouchingSpeed">Speed at which the character will try to move while crouching with a support that provides traction.
		/// Relative velocities with a greater magnitude will be decelerated.</param>
		/// <param name="proneSpeed">Speed at which the character will try to move while prone with a support that provides traction.
		/// Relative velocities with a greater magnitude will be decelerated.</param>
		/// <param name="tractionForce">Maximum force that the character can apply while on a support which provides traction.</param>
		/// <param name="slidingSpeed">Speed at which the character will try to move while on a support that does not provide traction.
		/// Relative velocities with a greater magnitude will be decelerated.</param>
		/// <param name="slidingForce">Maximum force that the character can apply while on a support which does not provide traction</param>
		/// <param name="airSpeed">Speed at which the character will try to move with no support.
		/// The character will not be decelerated while airborne.</param>
		/// <param name="airForce">Maximum force that the character can apply with no support.</param>
		/// <param name="jumpSpeed">Speed at which the character leaves the ground when it jumps</param>
		/// <param name="slidingJumpSpeed">Speed at which the character leaves the ground when it jumps without traction</param>
		/// <param name="maximumGlueForce">Maximum force the vertical motion constraint is allowed to apply in an attempt to keep the character on the ground.</param>
		public CharacterController(
			// Fix64 cannot be used for default parameters. As a workaround, make all parameters nullable and assign default values inside the constructor
			Vector3 position = new Vector3(),
			Fix64? height = null, Fix64? crouchingHeight = null, Fix64? proneHeight = null, Fix64? radius = null, Fix64? margin = null, Fix64? mass = null,
            Fix64? maximumTractionSlope = null, Fix64? maximumSupportSlope = null,
            Fix64? standingSpeed = null, Fix64? crouchingSpeed = null, Fix64? proneSpeed = null, Fix64? tractionForce = null, Fix64? slidingSpeed = null, Fix64? slidingForce = null, Fix64? airSpeed = null, Fix64? airForce = null,
            Fix64? jumpSpeed = null, Fix64? slidingJumpSpeed = null,
            Fix64? maximumGlueForce = null
			)
        {
			if (height == null)
				height = (Fix64)1.7m;
			if (crouchingHeight == null)
				crouchingHeight = (Fix64)(1.7m * .7m);
			if (proneHeight == null)
				proneHeight = (Fix64)(1.7m * 0.3m);
			if (radius == null)
				radius = (Fix64)0.6m;
			if (margin == null)
				margin = (Fix64)0.1m;
			if (mass == null)
				mass = 10;
			if (maximumTractionSlope == null)
				maximumTractionSlope = (Fix64)0.8m;
			if (maximumSupportSlope == null)
				maximumSupportSlope = (Fix64)1.3m;
			if (standingSpeed == null)
				standingSpeed = 8;
			if (crouchingSpeed == null)
				crouchingSpeed = 3;
			if (proneSpeed == null)
				proneSpeed = (Fix64)1.5m;
			if (tractionForce == null)
				tractionForce = 1000;
			if (slidingSpeed == null)
				slidingSpeed = 6;
			if (slidingForce == null)
				slidingForce = 50;
			if (airSpeed == null)
				airSpeed = 1;
			if (airForce == null)
				airForce = 250;
			if (jumpSpeed == null)
				jumpSpeed = (Fix64)4.5m;
			if (slidingJumpSpeed == null)
				slidingJumpSpeed = 3;
			if (maximumGlueForce == null)
				maximumGlueForce = 5000;

			if (margin > radius || margin > crouchingHeight || margin > height)
                throw new ArgumentException("Margin must not be larger than the character's radius or height.");

            Body = new Cylinder(position, (Fix64)height, (Fix64)radius, (Fix64)mass);
            Body.IgnoreShapeChanges = true; //Wouldn't want inertia tensor recomputations to occur when crouching and such.
            Body.CollisionInformation.Shape.CollisionMargin = (Fix64)margin;
            //Making the character a continuous object prevents it from flying through walls which would be pretty jarring from a player's perspective.
            Body.PositionUpdateMode = PositionUpdateMode.Continuous;
            Body.LocalInertiaTensorInverse = new Matrix3x3();
            //TODO: In v0.16.2, compound bodies would override the material properties that get set in the CreatingPair event handler.
            //In a future version where this is changed, change this to conceptually minimally required CreatingPair.
            Body.CollisionInformation.Events.DetectingInitialCollision += RemoveFriction;
            Body.LinearDamping = F64.C0;
            ContactCategorizer = new CharacterContactCategorizer((Fix64)maximumTractionSlope, (Fix64)maximumSupportSlope);
            QueryManager = new QueryManager(Body, ContactCategorizer);
            SupportFinder = new SupportFinder(Body, QueryManager, ContactCategorizer);
            HorizontalMotionConstraint = new HorizontalMotionConstraint(Body, SupportFinder);
            HorizontalMotionConstraint.PositionAnchorDistanceThreshold = (Fix64)radius * F64.C0p25;
            VerticalMotionConstraint = new VerticalMotionConstraint(Body, SupportFinder, (Fix64)maximumGlueForce);
            StepManager = new StepManager(Body, ContactCategorizer, SupportFinder, QueryManager, HorizontalMotionConstraint);
            StanceManager = new StanceManager(Body, (Fix64)crouchingHeight, (Fix64)proneHeight, QueryManager, SupportFinder);
            PairLocker = new CharacterPairLocker(Body);

            StandingSpeed = (Fix64)standingSpeed;
            CrouchingSpeed = (Fix64)crouchingSpeed;
            ProneSpeed = (Fix64)proneSpeed;
            TractionForce = (Fix64)tractionForce;
            SlidingSpeed = (Fix64)slidingSpeed;
            SlidingForce = (Fix64)slidingForce;
            AirSpeed = (Fix64)airSpeed;
            AirForce = (Fix64)airForce;
            JumpSpeed = (Fix64)jumpSpeed;
            SlidingJumpSpeed = (Fix64)slidingJumpSpeed;

            //Enable multithreading for the characters.  
            IsUpdatedSequentially = false;
            //Link the character body to the character controller so that it can be identified by the locker.
            //Any object which replaces this must implement the ICharacterTag for locking to work properly.
            Body.CollisionInformation.Tag = new CharacterSynchronizer(Body);
        }




        void RemoveFriction(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            var collidablePair = pair as CollidablePairHandler;
            if (collidablePair != null)
            {
                //The default values for InteractionProperties is all zeroes- zero friction, zero bounciness.
                //That's exactly how we want the character to behave when hitting objects.
                collidablePair.UpdateMaterialProperties(new InteractionProperties());
            }
        }


        /// <summary>
        /// Cylinder shape used to compute the expanded bounding box of the character.
        /// </summary>
        void ExpandBoundingBox()
        {
            if (Body.ActivityInformation.IsActive)
            {
                //This runs after the bounding box updater is run, but before the broad phase.
                //Expanding the character's bounding box ensures that minor variations in velocity will not cause
                //any missed information.

                //TODO: seems a bit silly to do this work sequentially. Would be better if it could run in parallel in the proper location.

                var down = Down;
                var boundingBox = Body.CollisionInformation.BoundingBox;
                //Expand the bounding box up and down using the step height.
                Vector3 expansion;
                Vector3.Multiply(ref down, StepManager.MaximumStepHeight, out expansion);
                expansion.X = Fix64.Abs(expansion.X);
                expansion.Y = Fix64.Abs(expansion.Y);
                expansion.Z = Fix64.Abs(expansion.Z);

                //When the character climbs a step, it teleports horizontally a little to gain support. Expand the bounding box to accommodate the margin.
                //Compute the expansion caused by the extra radius along each axis.
                //There's a few ways to go about doing this.

                //The following is heavily cooked, but it is based on the angle between the vertical axis and a particular axis.
                //Given that, the amount of the radial expansion required along that axis can be computed.
                //The dot product would provide the cos(angle) between the vertical axis and a chosen axis.
                //Equivalently, it is how much expansion would be along that axis, if the vertical axis was the axis of expansion.
                //However, it's not. The dot product actually gives us the expansion along an axis perpendicular to the chosen axis, pointing away from the character's vertical axis.

                //What we need is actually given by the sin(angle), which is given by ||verticalAxis x testAxis||.
                //The sin(angle) is the projected length of the verticalAxis (not the expansion!) on the axis perpendicular to the testAxis pointing away from the character's vertical axis.
                //That projected length, however is equal to the expansion along the test axis, which is exactly what we want.
                //To show this, try setting up the triangles at the corner of a cylinder with the world axes and cylinder axes.

                //Since the test axes we're using are all standard directions ({0,0,1}, {0,1,0}, and {0,0,1}), most of the cross product logic simplifies out, and we are left with:
                var horizontalExpansionAmount = Body.CollisionInformation.Shape.CollisionMargin * F64.C1p1;
                Vector3 squaredDown;
                squaredDown.X = down.X * down.X;
                squaredDown.Y = down.Y * down.Y;
                squaredDown.Z = down.Z * down.Z;
                expansion.X += horizontalExpansionAmount * Fix64.Sqrt(squaredDown.Y + squaredDown.Z);
                expansion.Y += horizontalExpansionAmount * Fix64.Sqrt(squaredDown.X + squaredDown.Z);
                expansion.Z += horizontalExpansionAmount * Fix64.Sqrt(squaredDown.X + squaredDown.Y);

                Vector3.Add(ref expansion, ref boundingBox.Max, out boundingBox.Max);
                Vector3.Subtract(ref boundingBox.Min, ref expansion, out boundingBox.Min);

                Body.CollisionInformation.BoundingBox = boundingBox;

            }


        }


        void IBeforeSolverUpdateable.Update(Fix64 dt)
        {
            //Someone may want to use the Body.CollisionInformation.Tag for their own purposes.
            //That could screw up the locking mechanism above and would be tricky to track down.
            //Consider using the making the custom tag implement ICharacterTag, modifying LockCharacterPairs to analyze the different Tag type, or using the Entity.Tag for the custom data instead.
            Debug.Assert(Body.CollisionInformation.Tag is ICharacterTag, "The character.Body.CollisionInformation.Tag must implement ICharacterTag to link the CharacterController and its body together for character-related locking to work in multithreaded simulations.");

            SupportData supportData;

            HorizontalMotionConstraint.UpdateMovementBasis(ref viewDirection);
            //We can't let multiple characters manage the same pairs simultaneously.  Lock it up!
            PairLocker.LockCharacterPairs();
            try
            {
                CorrectContacts();

                bool hadSupport = SupportFinder.HasSupport;

                SupportFinder.UpdateSupports(ref HorizontalMotionConstraint.movementDirection3d);
                supportData = SupportFinder.SupportData;


                //Compute the initial velocities relative to the support.
                Vector3 relativeVelocity;
                ComputeRelativeVelocity(ref supportData, out relativeVelocity);
                Fix64 verticalVelocity = Vector3.Dot(supportData.Normal, relativeVelocity);


                //Don't attempt to use an object as support if we are flying away from it (and we were never standing on it to begin with).
                if (SupportFinder.HasSupport && !hadSupport && verticalVelocity < F64.C0)
                {
                    SupportFinder.ClearSupportData();
                    supportData = new SupportData();
                }


                //Attempt to jump.
                if (TryToJump)
                {
                    //In the following, note that the jumping velocity changes are computed such that the separating velocity is specifically achieved,
                    //rather than just adding some speed along an arbitrary direction.  This avoids some cases where the character could otherwise increase
                    //the jump speed, which may not be desired.
                    if (SupportFinder.HasTraction)
                    {
                        //The character has traction, so jump straight up.
                        Fix64 currentDownVelocity = Vector3.Dot(Down, relativeVelocity);
                        //Target velocity is JumpSpeed.
                        Fix64 velocityChange = MathHelper.Max(jumpSpeed + currentDownVelocity, F64.C0);
                        ApplyJumpVelocity(ref supportData, Down * -velocityChange, ref relativeVelocity);


                        //Prevent any old contacts from hanging around and coming back with a negative depth.
                        foreach (var pair in Body.CollisionInformation.Pairs)
                            pair.ClearContacts();
                        SupportFinder.ClearSupportData();
                        supportData = new SupportData();
                    }
                    else if (SupportFinder.HasSupport)
                    {
                        //The character does not have traction, so jump along the surface normal instead.
                        Fix64 currentNormalVelocity = Vector3.Dot(supportData.Normal, relativeVelocity);
                        //Target velocity is JumpSpeed.
                        Fix64 velocityChange = MathHelper.Max(slidingJumpSpeed - currentNormalVelocity, F64.C0);
                        ApplyJumpVelocity(ref supportData, supportData.Normal * -velocityChange, ref relativeVelocity);

                        //Prevent any old contacts from hanging around and coming back with a negative depth.
                        foreach (var pair in Body.CollisionInformation.Pairs)
                            pair.ClearContacts();
                        SupportFinder.ClearSupportData();
                        supportData = new SupportData();
                    }
                }
                TryToJump = false;


                //Try to step!
                Vector3 newPosition;
                //Note: downstepping is often not required.
                //It's only really there for games that expect to be able to run down stairs at 40 miles an hour without zipping off into the void.
                //Most of the time, you can just comment out downstepping, and so long as the character is running at a reasonable speed,
                //gravity will do the work.

                //If your game would work without teleportation-based downstepping, it's probably a good idea to comment it out.
                //Downstepping can be fairly expensive.

                //You can also avoid doing upstepping by fattening up the character's margin, turning it into more of a capsule.
                //Instead of teleporting up steps, it would slide up.
                //Without teleportation-based upstepping, steps usually need to be quite a bit smaller (i.e. fairly normal sized, instead of 2 feet tall).
                if (StepManager.TryToStepDown(out newPosition) ||
                    StepManager.TryToStepUp(out newPosition))
                {
                    supportData = TeleportToPosition(newPosition, dt);
                }

                if (StanceManager.UpdateStance(out newPosition))
                {
                    supportData = TeleportToPosition(newPosition, dt);
                }
            }
            finally
            {
                PairLocker.UnlockCharacterPairs();
            }

            //Tell the constraints to get ready to solve.
            HorizontalMotionConstraint.UpdateSupportData();
            VerticalMotionConstraint.UpdateSupportData();

            //Update the horizontal motion constraint's state.
            if (supportData.SupportObject != null)
            {
                Fix64 speed;
                switch (StanceManager.CurrentStance)
                {
                    case Stance.Prone:
                        speed = proneSpeed;
                        break;
                    case Stance.Crouching:
                        speed = crouchingSpeed;
                        break;
                    default:
                        speed = standingSpeed;
                        break;
                }
                if (SupportFinder.HasTraction)
                {
                    HorizontalMotionConstraint.MovementMode = MovementMode.Traction;
                    HorizontalMotionConstraint.TargetSpeed = speed;
                    HorizontalMotionConstraint.MaximumForce = tractionForce;
                }
                else
                {
                    HorizontalMotionConstraint.MovementMode = MovementMode.Sliding;
                    HorizontalMotionConstraint.TargetSpeed = MathHelper.Min(speed, slidingSpeed);
                    HorizontalMotionConstraint.MaximumForce = MathHelper.Min(tractionForce, slidingForce);
                }
            }
            else
            {
                HorizontalMotionConstraint.MovementMode = MovementMode.Floating;
                HorizontalMotionConstraint.TargetSpeed = airSpeed;
                HorizontalMotionConstraint.MaximumForce = airForce;
            }
            HorizontalMotionConstraint.TargetSpeed *= SpeedScale;


        }

        SupportData TeleportToPosition(Vector3 newPosition, Fix64 dt)
        {

            Body.Position = newPosition;
            var orientation = Body.Orientation;
            //The re-do of contacts won't do anything unless we update the collidable's world transform.
            Body.CollisionInformation.UpdateWorldTransform(ref newPosition, ref orientation);
            //Refresh all the narrow phase collisions.
            foreach (var pair in Body.CollisionInformation.Pairs)
            {
                //Clear out the old contacts.  This prevents contacts in persistent manifolds from surviving the step
                //Such old contacts might still have old normals which blocked the character's forward motion.

                pair.ClearContacts();
                pair.UpdateCollision(dt);

            }
            //Also re-collect supports.
            //This will ensure the constraint and other velocity affectors have the most recent information available.
            SupportFinder.UpdateSupports(ref HorizontalMotionConstraint.movementDirection3d);
            return SupportFinder.SupportData;
        }

        void CorrectContacts()
        {
            //Go through the contacts associated with the character.
            //If the contact is at the bottom of the character, regardless of its normal, take a closer look.
            //If the direction from the closest point on the inner cylinder to the contact position has traction
            //and the contact's normal does not, then replace the contact normal with the offset direction.

            //This is necessary because various convex pair manifolds use persistent manifolds.
            //Contacts in these persistent manifolds can live too long for the character to behave perfectly
            //when going over (usually tiny) steps.

            Vector3 downDirection = Body.OrientationMatrix.Down;
            Vector3 position = Body.Position;
            Fix64 margin = Body.CollisionInformation.Shape.CollisionMargin;
            Fix64 minimumHeight = Body.Height * F64.C0p5 - margin;
            Fix64 coreRadius = Body.Radius - margin;
            Fix64 coreRadiusSquared = coreRadius * coreRadius;
            foreach (var pair in Body.CollisionInformation.Pairs)
            {
                foreach (var contactData in pair.Contacts)
                {
                    var contact = contactData.Contact;
                    Fix64 dot;
                    //Check to see if the contact position is at the bottom of the character.
                    Vector3 offset = contact.Position - Body.Position;
                    Vector3.Dot(ref offset, ref downDirection, out dot);
                    if (dot > minimumHeight)
                    {

                        //It is a 'bottom' contact!
                        //So, compute the offset from the inner cylinder to the contact.
                        //To do this, compute the closest point on the inner cylinder.
                        //Since we know it's on the bottom, all we need is to compute the horizontal offset.
                        Vector3.Dot(ref offset, ref downDirection, out dot);
                        Vector3 horizontalOffset;
                        Vector3.Multiply(ref downDirection, dot, out horizontalOffset);
                        Vector3.Subtract(ref offset, ref horizontalOffset, out horizontalOffset);
                        Fix64 length = horizontalOffset.LengthSquared();
                        if (length > coreRadiusSquared)
                        {
                            //It's beyond the edge of the cylinder; clamp it.
                            Vector3.Multiply(ref horizontalOffset, coreRadius / Fix64.Sqrt(length), out horizontalOffset);
                        }
                        //It's on the bottom, so add the bottom height.
                        Vector3 closestPointOnCylinder;
                        Vector3.Multiply(ref downDirection, minimumHeight, out closestPointOnCylinder);
                        Vector3.Add(ref closestPointOnCylinder, ref horizontalOffset, out closestPointOnCylinder);
                        Vector3.Add(ref closestPointOnCylinder, ref position, out closestPointOnCylinder);

                        //Compute the offset from the cylinder to the offset.
                        Vector3 offsetDirection;
                        Vector3.Subtract(ref contact.Position, ref closestPointOnCylinder, out offsetDirection);
                        length = offsetDirection.LengthSquared();
                        if (length > Toolbox.Epsilon)
                        {
                            //Normalize the offset.
                            Vector3.Divide(ref offsetDirection, Fix64.Sqrt(length), out offsetDirection);
                        }
                        else
                            continue; //If there's no offset, it's really deep and correcting this contact might be a bad idea.

                        Vector3.Dot(ref offsetDirection, ref downDirection, out dot);
                        Fix64 dotOriginal;
                        Vector3.Dot(ref contact.Normal, ref downDirection, out dotOriginal);
                        if (dot > Fix64.Abs(dotOriginal)) //if the new offsetDirection normal is less steep than the original slope...
                        {
                            //Then use it!
                            Vector3.Dot(ref offsetDirection, ref contact.Normal, out dot);
                            if (dot < F64.C0)
                            {
                                //Don't flip the normal relative to the contact normal.  That would be bad!
                                Vector3.Negate(ref offsetDirection, out offsetDirection);
                                dot = -dot;
                            }
                            //Update the contact data using the corrected information.
                            //The penetration depth is conservatively updated; it will be less than or equal to the 'true' depth in this direction.
                            contact.PenetrationDepth *= dot;
                            contact.Normal = offsetDirection;
                        }
                    }
                }
            }

        }

        void ComputeRelativeVelocity(ref SupportData supportData, out Vector3 relativeVelocity)
        {

            //Compute the relative velocity between the body and its support, if any.
            //The relative velocity will be updated as impulses are applied.
            relativeVelocity = Body.LinearVelocity;
            if (SupportFinder.HasSupport)
            {
                //Only entities have velocity.
                var entityCollidable = supportData.SupportObject as EntityCollidable;
                if (entityCollidable != null)
                {
                    //It's possible for the support's velocity to change due to another character jumping if the support is dynamic.
                    //Don't let that happen while the character is computing a relative velocity!
                    Vector3 entityVelocity;
                    bool locked = entityCollidable.Entity.IsDynamic;
                    if (locked)
                        entityCollidable.Entity.Locker.Enter();
                    try
                    {
                        entityVelocity = Toolbox.GetVelocityOfPoint(supportData.Position, entityCollidable.Entity.Position, entityCollidable.Entity.LinearVelocity, entityCollidable.Entity.AngularVelocity);
                    }
                    finally
                    {
                        if (locked)
                            entityCollidable.Entity.Locker.Exit();
                    }
                    Vector3.Subtract(ref relativeVelocity, ref entityVelocity, out relativeVelocity);
                }
            }

        }

        /// <summary>
        /// Changes the relative velocity between the character and its support.
        /// </summary>
        /// <param name="supportData">Support data to use to jump.</param>
        /// <param name="velocityChange">Change to apply to the character and support relative velocity.</param>
        /// <param name="relativeVelocity">Relative velocity to update.</param>
        void ApplyJumpVelocity(ref SupportData supportData, Vector3 velocityChange, ref Vector3 relativeVelocity)
        {
            Body.LinearVelocity += velocityChange;
            var entityCollidable = supportData.SupportObject as EntityCollidable;
            if (entityCollidable != null)
            {
                if (entityCollidable.Entity.IsDynamic)
                {
                    Vector3 change = velocityChange * jumpForceFactor;
                    //Multiple characters cannot attempt to modify another entity's velocity at the same time.
                    entityCollidable.Entity.Locker.Enter();
                    try
                    {
                        entityCollidable.Entity.LinearMomentum += change * -Body.Mass;
                    }
                    finally
                    {
                        entityCollidable.Entity.Locker.Exit();
                    }
                    velocityChange += change;
                }
            }

            //Update the relative velocity as well.  It's a ref parameter, so this update will be reflected in the calling scope.
            Vector3.Add(ref relativeVelocity, ref velocityChange, out relativeVelocity);

        }

        /// <summary>
        /// <para>Gets or sets whether the character should attempt to jump during the next update. During each update, this flag will be set to false.
        /// If it has traction, it will go straight up. If it doesn't have traction, but is still supported by something, it will jump in the direction of the surface normal.</para>
        /// <para>Setting this to true has the same effect as calling Jump. This property is primarily useful for fully resetting the physical state to avoid desynchronization, e.g. in networking.</para>
        /// </summary>
        public bool TryToJump
        {
            get;
            set;
        }

        /// <summary>
        /// <para>Jumps the character off of whatever it's currently standing on during the next update.  If it has traction, it will go straight up.
        /// If it doesn't have traction, but is still supported by something, it will jump in the direction of the surface normal.</para>
        /// <para>The same effect can be achieved by setting TryToJump to true.</para>
        /// </summary>
        public void Jump()
        {
            //The actual jump velocities are applied next frame.  This ensures that gravity doesn't pre-emptively slow the jump, and uses more
            //up-to-date support data.
            TryToJump = true;
        }

        public override void OnAdditionToSpace(Space newSpace)
        {
            //Add any supplements to the space too.
            newSpace.Add(Body);
            newSpace.Add(HorizontalMotionConstraint);
            newSpace.Add(VerticalMotionConstraint);
            //This character controller requires the standard implementation of Space.
            newSpace.BoundingBoxUpdater.Finishing += ExpandBoundingBox;

            Body.AngularVelocity = new Vector3();
            Body.LinearVelocity = new Vector3();
        }
        public override void OnRemovalFromSpace(Space oldSpace)
        {
            //Remove any supplements from the space too.
            oldSpace.Remove(Body);
            oldSpace.Remove(HorizontalMotionConstraint);
            oldSpace.Remove(VerticalMotionConstraint);
            //This character controller requires the standard implementation of Space.
            oldSpace.BoundingBoxUpdater.Finishing -= ExpandBoundingBox;
            SupportFinder.ClearSupportData();
            Body.AngularVelocity = new Vector3();
            Body.LinearVelocity = new Vector3();
        }


    }
}


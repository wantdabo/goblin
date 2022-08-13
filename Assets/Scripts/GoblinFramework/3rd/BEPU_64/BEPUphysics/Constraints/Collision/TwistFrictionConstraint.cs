using System;
using BEPUphysics.Entities;
 
using BEPUphysics.Settings;
using BEPUutilities;
using BEPUutilities.DataStructures;
using FixMath.NET;

namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces necessary to slow down and stop twisting motion in a collision between two entities.
    /// </summary>
    public class TwistFrictionConstraint : SolverUpdateable
    {
        private readonly Fix64[] leverArms = new Fix64[4];
        private ConvexContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the contact manifold constraint that owns this constraint.
        ///</summary>
        public ConvexContactManifoldConstraint ContactManifoldConstraint { get { return contactManifoldConstraint; } }
        internal Fix64 accumulatedImpulse;
        private Fix64 angularX, angularY, angularZ;
        private int contactCount;
        private Fix64 friction;
        Entity entityA, entityB;
        bool entityADynamic, entityBDynamic;
        private Fix64 velocityToImpulse;

        ///<summary>
        /// Constructs a new twist friction constraint.
        ///</summary>
        public TwistFrictionConstraint()
        {
            isActive = false;
        }

        /// <summary>
        /// Gets the torque applied by twist friction.
        /// </summary>
        public Fix64 TotalTorque
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the angular velocity between the associated entities.
        ///</summary>
        public Fix64 RelativeVelocity
        {
            get
            {
                Fix64 lambda = F64.C0;
                if (entityA != null)
                    lambda = entityA.angularVelocity.X * angularX + entityA.angularVelocity.Y * angularY + entityA.angularVelocity.Z * angularZ;
                if (entityB != null)
                    lambda -= entityB.angularVelocity.X * angularX + entityB.angularVelocity.Y * angularY + entityB.angularVelocity.Z * angularZ;
                return lambda;
            }
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override Fix64 SolveIteration()
        {
            //Compute relative velocity.  Collisions can occur between an entity and a non-entity.  If it's not an entity, assume it's not moving.
            Fix64 lambda = RelativeVelocity;
            
            lambda *= velocityToImpulse; //convert to impulse

            //Clamp accumulated impulse
            Fix64 previousAccumulatedImpulse = accumulatedImpulse;
            Fix64 maximumFrictionForce = F64.C0;
            for (int i = 0; i < contactCount; i++)
            {
                maximumFrictionForce += leverArms[i] * contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
            }
            maximumFrictionForce *= friction;
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + lambda, -maximumFrictionForce, maximumFrictionForce); //instead of maximumFrictionForce, could recompute each iteration...
            lambda = accumulatedImpulse - previousAccumulatedImpulse;


            //Apply the impulse
#if !WINDOWS
            Vector3 angular = new Vector3();
#else
            Vector3 angular;
#endif
            angular.X = lambda * angularX;
            angular.Y = lambda * angularY;
            angular.Z = lambda * angularZ;
            if (entityADynamic)
            {
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                angular.X = -angular.X;
                angular.Y = -angular.Y;
                angular.Z = -angular.Z;
                entityB.ApplyAngularImpulse(ref angular);
            }


            return Fix64.Abs(lambda);
        }


        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(Fix64 dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            //Compute the jacobian......  Real hard!
            Vector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
            angularX = normal.X;
            angularY = normal.Y;
            angularZ = normal.Z;

            //Compute inverse effective mass matrix
            Fix64 entryA, entryB;

            //these are the transformed coordinates
            Fix64 tX, tY, tZ;
            if (entityADynamic)
            {
                tX = angularX * entityA.inertiaTensorInverse.M11 + angularY * entityA.inertiaTensorInverse.M21 + angularZ * entityA.inertiaTensorInverse.M31;
                tY = angularX * entityA.inertiaTensorInverse.M12 + angularY * entityA.inertiaTensorInverse.M22 + angularZ * entityA.inertiaTensorInverse.M32;
                tZ = angularX * entityA.inertiaTensorInverse.M13 + angularY * entityA.inertiaTensorInverse.M23 + angularZ * entityA.inertiaTensorInverse.M33;
                entryA = tX * angularX + tY * angularY + tZ * angularZ + entityA.inverseMass;
            }
            else
                entryA = F64.C0;

            if (entityBDynamic)
            {
                tX = angularX * entityB.inertiaTensorInverse.M11 + angularY * entityB.inertiaTensorInverse.M21 + angularZ * entityB.inertiaTensorInverse.M31;
                tY = angularX * entityB.inertiaTensorInverse.M12 + angularY * entityB.inertiaTensorInverse.M22 + angularZ * entityB.inertiaTensorInverse.M32;
                tZ = angularX * entityB.inertiaTensorInverse.M13 + angularY * entityB.inertiaTensorInverse.M23 + angularZ * entityB.inertiaTensorInverse.M33;
                entryB = tX * angularX + tY * angularY + tZ * angularZ + entityB.inverseMass;
            }
            else
                entryB = F64.C0;

            velocityToImpulse = -1 / (entryA + entryB);


            //Compute the relative velocity to determine what kind of friction to use
            Fix64 relativeAngularVelocity = RelativeVelocity;
            //Set up friction and find maximum friction force
            Vector3 relativeSlidingVelocity = contactManifoldConstraint.SlidingFriction.relativeVelocity;
            friction = Fix64.Abs(relativeAngularVelocity) > CollisionResponseSettings.StaticFrictionVelocityThreshold ||
					   Fix64.Abs(relativeSlidingVelocity.X) + Fix64.Abs(relativeSlidingVelocity.Y) + Fix64.Abs(relativeSlidingVelocity.Z) > CollisionResponseSettings.StaticFrictionVelocityThreshold
                           ? contactManifoldConstraint.materialInteraction.KineticFriction
                           : contactManifoldConstraint.materialInteraction.StaticFriction;
            friction *= CollisionResponseSettings.TwistFrictionFactor;

            contactCount = contactManifoldConstraint.penetrationConstraints.Count;

            Vector3 contactOffset;
            for (int i = 0; i < contactCount; i++)
            {
                Vector3.Subtract(ref contactManifoldConstraint.penetrationConstraints.Elements[i].contact.Position, ref contactManifoldConstraint.SlidingFriction.manifoldCenter, out contactOffset);
                leverArms[i] = contactOffset.Length();
            }



        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Apply the warmstarting impulse.
#if !WINDOWS
            Vector3 angular = new Vector3();
#else
            Vector3 angular;
#endif
            angular.X = accumulatedImpulse * angularX;
            angular.Y = accumulatedImpulse * angularY;
            angular.Z = accumulatedImpulse * angularZ;
            if (entityADynamic)
            {
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                angular.X = -angular.X;
                angular.Y = -angular.Y;
                angular.Z = -angular.Z;
                entityB.ApplyAngularImpulse(ref angular);
            }
        }

        internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            isActive = true;

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        internal void CleanUp()
        {
            accumulatedImpulse = F64.C0;
            contactManifoldConstraint = null;
            entityA = null;
            entityB = null;
            isActive = false;
        }

        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            //This should never really have to be called.
            if (entityA != null)
                outputInvolvedEntities.Add(entityA);
            if (entityB != null)
                outputInvolvedEntities.Add(entityB);
        }
     


    }
}
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Cone-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Cone : Entity<ConvexCollidable<ConeShape>>
    {
        /// <summary>
        /// Gets or sets the length of the cone.
        /// </summary>
        public Fix64 Height
        {
            get
            {
                return CollisionInformation.Shape.Height;
            }
            set
            {
                CollisionInformation.Shape.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the cone.
        /// </summary>
        public Fix64 Radius
        {
            get
            {
                return CollisionInformation.Shape.Radius;
            }
            set
            {
                CollisionInformation.Shape.Radius = value;
            }
        }


        private Cone(Fix64 high, Fix64 rad)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)))
        {
        }

        private Cone(Fix64 high, Fix64 rad, Fix64 mass)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)), mass)
        {
        }



        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(Vector3 position, Fix64 height, Fix64 radius, Fix64 mass)
            : this(height, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(Vector3 position, Fix64 height, Fix64 radius)
            : this(height, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(MotionState motionState, Fix64 height, Fix64 radius, Fix64 mass)
            : this(height, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(MotionState motionState, Fix64 height, Fix64 radius)
            : this(height, radius)
        {
            MotionState = motionState;
        }

 


    }
}
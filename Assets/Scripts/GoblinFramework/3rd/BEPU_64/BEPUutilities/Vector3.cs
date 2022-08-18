using FixMath.NET;
using System;

namespace BEPUutilities
{
    /// <summary>
    /// Provides XNA-like 3D vector math.
    /// </summary>
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public Fix64 X;
        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public Fix64 Y;
        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public Fix64 Z;

        /// <summary>
        /// A vector with components (0,0,0);
        /// </summary>
        public static readonly Vector3 zero;
        /// <summary>
        /// A vector with components (-1,0,0);
        /// </summary>
        public static readonly Vector3 left;
        /// <summary>
        /// A vector with components (1,0,0);
        /// </summary>
        public static readonly Vector3 right;
        /// <summary>
        /// A vector with components (0,1,0);
        /// </summary>
        public static readonly Vector3 up;
        /// <summary>
        /// A vector with components (0,-1,0);
        /// </summary>
        public static readonly Vector3 down;
        /// <summary>
        /// A vector with components (0,0,-1);
        /// </summary>
        public static readonly Vector3 back;
        /// <summary>
        /// A vector with components (0,0,1);
        /// </summary>
        public static readonly Vector3 forward;
        /// <summary>
        /// A vector with components (1,1,1);
        /// </summary>
        public static readonly Vector3 one;
        /// <summary>
        /// A vector with components 
        /// (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly Vector3 MinValue;
        /// <summary>
        /// A vector with components 
        /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly Vector3 MaxValue;

        static Vector3()
        {
            one = new Vector3(1, 1, 1);
            zero = new Vector3(0, 0, 0);
            left = new Vector3(-1, 0, 0);
            right = new Vector3(1, 0, 0);
            up = new Vector3(0, 1, 0);
            down = new Vector3(0, -1, 0);
            back = new Vector3(0, 0, -1);
            forward = new Vector3(0, 0, 1);
            MinValue = new Vector3(Fix64.MinValue);
            MaxValue = new Vector3(Fix64.MaxValue);
        }

        /// <summary>
        /// Constructs a new 3d vector.
        /// </summary>
        /// <param name="x">X component of the vector.</param>
        /// <param name="y">Y component of the vector.</param>
        /// <param name="z">Z component of the vector.</param>
        public Vector3(Fix64 x, Fix64 y, Fix64 z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Constructs a new 3d vector.
        /// </summary>
        /// <param name="val">val component of the (val, val, val)vector.</param>
        public Vector3(Fix64 val) : this(val, val, val) { }

        /// <summary>
        /// Constructs a new 3d vector.
        /// </summary>
        /// <param name="xy">X and Y components of the vector.</param>
        /// <param name="z">Z component of the vector.</param>
        public Vector3(Vector2 xy, Fix64 z)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
        }

        /// <summary>
        /// Constructs a new 3d vector.
        /// </summary>
        /// <param name="x">X component of the vector.</param>
        /// <param name="yz">Y and Z components of the vector.</param>
        public Vector3(Fix64 x, Vector2 yz)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
        }

        /// <summary>
        /// Computes the squared length of the vector.
        /// </summary>
        /// <returns>Squared length of the vector.</returns>
        public Fix64 LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Computes the length of the vector.
        /// </summary>
        /// <returns>Length of the vector.</returns>
        public Fix64 Length()
        {
            return Fix64.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            Fix64 inverse = F64.C1 / Fix64.Sqrt(X * X + Y * Y + Z * Z);
            X *= inverse;
            Y *= inverse;
            Z *= inverse;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public Vector4 ToVector4() 
        {
            return ToVector4(0);
        }

        public Vector4 ToVector4(Fix64 w)
        {
            return new Vector4(X, Y, Z, w);
        }

        /// <summary>
        /// Gets a string representation of the vector.
        /// </summary>
        /// <returns>String representing the vector.</returns>
        public override string ToString()
        {
            return "{" + X + ", " + Y + ", " + Z + "}";
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="a">First vector in the product.</param>
        /// <param name="b">Second vector in the product.</param>
        /// <returns>Resulting dot product.</returns>
        public static Fix64 Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="a">First vector in the product.</param>
        /// <param name="b">Second vector in the product.</param>
        /// <param name="product">Resulting dot product.</param>
        public static void Dot(ref Vector3 a, ref Vector3 b, out Fix64 product)
        {
            product = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="a">First vector to add.</param>
        /// <param name="b">Second vector to add.</param>
        /// <param name="sum">Sum of the two vectors.</param>
        public static void Add(ref Vector3 a, ref Vector3 b, out Vector3 sum)
        {
            sum.X = a.X + b.X;
            sum.Y = a.Y + b.Y;
            sum.Z = a.Z + b.Z;
        }
        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">Vector to subtract from.</param>
        /// <param name="b">Vector to subtract from the first vector.</param>
        /// <param name="difference">Result of the subtraction.</param>
        public static void Subtract(ref Vector3 a, ref Vector3 b, out Vector3 difference)
        {
            difference.X = a.X - b.X;
            difference.Y = a.Y - b.Y;
            difference.Z = a.Z - b.Z;
        }
        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="scale">Amount to scale.</param>
        /// <param name="result">Scaled vector.</param>
        public static void Multiply(ref Vector3 v, Fix64 scale, out Vector3 result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
            result.Z = v.Z * scale;
        }

        /// <summary>
        /// Multiplies two vectors on a per-component basis.
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <param name="result">Result of the componentwise multiplication.</param>
        public static void Multiply(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
        }

        /// <summary>
        /// Divides a vector's components by some amount.
        /// </summary>
        /// <param name="v">Vector to divide.</param>
        /// <param name="divisor">Value to divide the vector's components.</param>
        /// <param name="result">Result of the division.</param>
        public static void Divide(ref Vector3 v, Fix64 divisor, out Vector3 result)
        {
            Fix64 inverse = F64.C1 / divisor;
            result.X = v.X * inverse;
            result.Y = v.Y * inverse;
            result.Z = v.Z * inverse;
        }
        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="f">Amount to scale.</param>
        /// <returns>Scaled vector.</returns>
        public static Vector3 operator *(Vector3 v, Fix64 f)
        {
            Vector3 toReturn;
            toReturn.X = v.X * f;
            toReturn.Y = v.Y * f;
            toReturn.Z = v.Z * f;
            return toReturn;
        }

        /// <summary>
        /// Scales a vector.
        /// </summary>
        /// <param name="v">Vector to scale.</param>
        /// <param name="f">Amount to scale.</param>
        /// <returns>Scaled vector.</returns>
        public static Vector3 operator *(Fix64 f, Vector3 v)
        {
            Vector3 toReturn;
            toReturn.X = v.X * f;
            toReturn.Y = v.Y * f;
            toReturn.Z = v.Z * f;
            return toReturn;
        }

        /// <summary>
        /// Multiplies two vectors on a per-component basis.
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <returns>Result of the componentwise multiplication.</returns>
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            Vector3 result;
            Multiply(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Divides a vector's components by some amount.
        /// </summary>
        /// <param name="v">Vector to divide.</param>
        /// <param name="f">Value to divide the vector's components.</param>
        /// <returns>Result of the division.</returns>
        public static Vector3 operator /(Vector3 v, Fix64 f)
        {
            Vector3 toReturn;
            f = F64.C1 / f;
            toReturn.X = v.X * f;
            toReturn.Y = v.Y * f;
            toReturn.Z = v.Z * f;
            return toReturn;
        }
        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">Vector to subtract from.</param>
        /// <param name="b">Vector to subtract from the first vector.</param>
        /// <returns>Result of the subtraction.</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Vector3 v;
            v.X = a.X - b.X;
            v.Y = a.Y - b.Y;
            v.Z = a.Z - b.Z;
            return v;
        }
        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="a">First vector to add.</param>
        /// <param name="b">Second vector to add.</param>
        /// <returns>Sum of the two vectors.</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            Vector3 v;
            v.X = a.X + b.X;
            v.Y = a.Y + b.Y;
            v.Z = a.Z + b.Z;
            return v;
        }


        /// <summary>
        /// Negates the vector.
        /// </summary>
        /// <param name="v">Vector to negate.</param>
        /// <returns>Negated vector.</returns>
        public static Vector3 operator -(Vector3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            return v;
        }
        /// <summary>
        /// Tests two vectors for componentwise equivalence.
        /// </summary>
        /// <param name="a">First vector to test for equivalence.</param>
        /// <param name="b">Second vector to test for equivalence.</param>
        /// <returns>Whether the vectors were equivalent.</returns>
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        /// <summary>
        /// Tests two vectors for componentwise inequivalence.
        /// </summary>
        /// <param name="a">First vector to test for inequivalence.</param>
        /// <param name="b">Second vector to test for inequivalence.</param>
        /// <returns>Whether the vectors were inequivalent.</returns>
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                return Equals((Vector3)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        /// <summary>
        /// Computes the squared distance between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="distanceSquared">Squared distance between the two vectors.</param>
        public static void DistanceSquared(ref Vector3 a, ref Vector3 b, out Fix64 distanceSquared)
        {
            Fix64 x = a.X - b.X;
            Fix64 y = a.Y - b.Y;
            Fix64 z = a.Z - b.Z;
            distanceSquared = x * x + y * y + z * z;
        }

        /// <summary>
        /// Computes the squared distance between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Squared distance between the two vectors.</returns>
        public static Fix64 DistanceSquared(Vector3 a, Vector3 b)
        {
            Fix64 x = a.X - b.X;
            Fix64 y = a.Y - b.Y;
            Fix64 z = a.Z - b.Z;
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Computes the distance between two two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="distance">Distance between the two vectors.</param>
        public static void Distance(ref Vector3 a, ref Vector3 b, out Fix64 distance)
        {
            Fix64 x = a.X - b.X;
            Fix64 y = a.Y - b.Y;
            Fix64 z = a.Z - b.Z;
            distance = Fix64.Sqrt(x * x + y * y + z * z);
        }
        /// <summary>
        /// Computes the distance between two two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Distance between the two vectors.</returns>
        public static Fix64 Distance(Vector3 a, Vector3 b)
        {
            Fix64 toReturn;
            Distance(ref a, ref b, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Gets the zero vector.
        /// </summary>
        public static Vector3 Zero
        {
            get
            {
                return new Vector3();
            }
        }

        /// <summary>
        /// Gets the up vector (0,1,0).
        /// </summary>
        public static Vector3 Up
        {
            get
            {
                return new Vector3()
                {
                    X = F64.C0,
                    Y = F64.C1,
                    Z = F64.C0
                };
            }
        }

        /// <summary>
        /// Gets the down vector (0,-1,0).
        /// </summary>
        public static Vector3 Down
        {
            get
            {
                return new Vector3()
                {
                    X = F64.C0,
                    Y = -1,
                    Z = F64.C0
                };
            }
        }

        /// <summary>
        /// Gets the right vector (1,0,0).
        /// </summary>
        public static Vector3 Right
        {
            get
            {
                return new Vector3()
                {
                    X = F64.C1,
                    Y = F64.C0,
                    Z = F64.C0
                };
            }
        }

        /// <summary>
        /// Gets the left vector (-1,0,0).
        /// </summary>
        public static Vector3 Left
        {
            get
            {
                return new Vector3()
                {
                    X = -1,
                    Y = F64.C0,
                    Z = F64.C0
                };
            }
        }

        /// <summary>
        /// Gets the forward vector (0,0,-1).
        /// </summary>
        public static Vector3 Forward
        {
            get
            {
                return new Vector3()
                {
                    X = F64.C0,
                    Y = F64.C0,
                    Z = -1
                };
            }
        }

        /// <summary>
        /// Gets the back vector (0,0,1).
        /// </summary>
        public static Vector3 Backward
        {
            get
            {
                return new Vector3()
                {
                    X = F64.C0,
                    Y = F64.C0,
                    Z = F64.C1
                };
            }
        }

        /// <summary>
        /// Gets a vector pointing along the X axis.
        /// </summary>
        public static Vector3 UnitX
        {
            get { return new Vector3 { X = F64.C1 }; }
        }

        /// <summary>
        /// Gets a vector pointing along the Y axis.
        /// </summary>
        public static Vector3 UnitY
        {
            get { return new Vector3 { Y = F64.C1 }; }
        }

        /// <summary>
        /// Gets a vector pointing along the Z axis.
        /// </summary>
        public static Vector3 UnitZ
        {
            get { return new Vector3 { Z = F64.C1 }; }
        }

        /// <summary>
        /// Computes the cross product between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Cross product of the two vectors.</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            Vector3 toReturn;
            Vector3.Cross(ref a, ref b, out toReturn);
            return toReturn;
        }
        /// <summary>
        /// Computes the cross product between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="result">Cross product of the two vectors.</param>
        public static void Cross(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            Fix64 resultX = a.Y * b.Z - a.Z * b.Y;
            Fix64 resultY = a.Z * b.X - a.X * b.Z;
            Fix64 resultZ = a.X * b.Y - a.Y * b.X;
            result.X = resultX;
            result.Y = resultY;
            result.Z = resultZ;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="v">Vector to normalize.</param>
        /// <returns>Normalized vector.</returns>
        public static Vector3 Normalize(Vector3 v)
        {
            Vector3 toReturn;
            Vector3.Normalize(ref v, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="v">Vector to normalize.</param>
        /// <param name="result">Normalized vector.</param>
        public static void Normalize(ref Vector3 v, out Vector3 result)
        {
            Fix64 inverse = F64.C1 / Fix64.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            result.X = v.X * inverse;
            result.Y = v.Y * inverse;
            result.Z = v.Z * inverse;
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="v">Vector to negate.</param>
        /// <param name="negated">Negated vector.</param>
        public static void Negate(ref Vector3 v, out Vector3 negated)
        {
            negated.X = -v.X;
            negated.Y = -v.Y;
            negated.Z = -v.Z;
        }

        /// <summary>
        /// Computes the absolute value of the input vector.
        /// </summary>
        /// <param name="v">Vector to take the absolute value of.</param>
        /// <param name="result">Vector with nonnegative elements.</param>
        public static void Abs(ref Vector3 v, out Vector3 result)
        {
            if (v.X < F64.C0)
                result.X = -v.X;
            else
                result.X = v.X;
            if (v.Y < F64.C0)
                result.Y = -v.Y;
            else
                result.Y = v.Y;
            if (v.Z < F64.C0)
                result.Z = -v.Z;
            else
                result.Z = v.Z;
        }

        /// <summary>
        /// Computes the absolute value of the input vector.
        /// </summary>
        /// <param name="v">Vector to take the absolute value of.</param>
        /// <returns>Vector with nonnegative elements.</returns>
        public static Vector3 Abs(Vector3 v)
        {
            Vector3 result;
            Abs(ref v, out result);
            return result;
        }

        /// <summary>
        /// Creates a vector from the lesser values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <param name="min">Vector containing the lesser values of each vector.</param>
        public static void Min(ref Vector3 a, ref Vector3 b, out Vector3 min)
        {
            min.X = a.X < b.X ? a.X : b.X;
            min.Y = a.Y < b.Y ? a.Y : b.Y;
            min.Z = a.Z < b.Z ? a.Z : b.Z;
        }

        /// <summary>
        /// Creates a vector from the lesser values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <returns>Vector containing the lesser values of each vector.</returns>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            Vector3 result;
            Min(ref a, ref b, out result);
            return result;
        }


        /// <summary>
        /// Creates a vector from the greater values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <param name="max">Vector containing the greater values of each vector.</param>
        public static void Max(ref Vector3 a, ref Vector3 b, out Vector3 max)
        {
            max.X = a.X > b.X ? a.X : b.X;
            max.Y = a.Y > b.Y ? a.Y : b.Y;
            max.Z = a.Z > b.Z ? a.Z : b.Z;
        }

        /// <summary>
        /// Creates a vector from the greater values in each vector.
        /// </summary>
        /// <param name="a">First input vector to compare values from.</param>
        /// <param name="b">Second input vector to compare values from.</param>
        /// <returns>Vector containing the greater values of each vector.</returns>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            Vector3 result;
            Max(ref a, ref b, out result);
            return result;
        }

        /// <summary>
        /// Computes an interpolated state between two vectors.
        /// </summary>
        /// <param name="start">Starting location of the interpolation.</param>
        /// <param name="end">Ending location of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end location to use.</param>
        /// <returns>Interpolated intermediate state.</returns>
        public static Vector3 Lerp(Vector3 start, Vector3 end, Fix64 interpolationAmount)
        {
            Vector3 toReturn;
            Lerp(ref start, ref end, interpolationAmount, out toReturn);
            return toReturn;
        }
        /// <summary>
        /// Computes an interpolated state between two vectors.
        /// </summary>
        /// <param name="start">Starting location of the interpolation.</param>
        /// <param name="end">Ending location of the interpolation.</param>
        /// <param name="interpolationAmount">Amount of the end location to use.</param>
        /// <param name="result">Interpolated intermediate state.</param>
        public static void Lerp(ref Vector3 start, ref Vector3 end, Fix64 interpolationAmount, out Vector3 result)
        {
            Fix64 startAmount = F64.C1 - interpolationAmount;
            result.X = start.X * startAmount + end.X * interpolationAmount;
            result.Y = start.Y * startAmount + end.Y * interpolationAmount;
            result.Z = start.Z * startAmount + end.Z * interpolationAmount;
        }

        /// <summary>
        /// Computes an intermediate location using hermite interpolation.
        /// </summary>
        /// <param name="value1">First position.</param>
        /// <param name="tangent1">Tangent associated with the first position.</param>
        /// <param name="value2">Second position.</param>
        /// <param name="tangent2">Tangent associated with the second position.</param>
        /// <param name="interpolationAmount">Amount of the second point to use.</param>
        /// <param name="result">Interpolated intermediate state.</param>
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, Fix64 interpolationAmount, out Vector3 result)
        {
            Fix64 weightSquared = interpolationAmount * interpolationAmount;
            Fix64 weightCubed = interpolationAmount * weightSquared;
            Fix64 value1Blend = F64.C2 * weightCubed - F64.C3 * weightSquared + F64.C1;
            Fix64 tangent1Blend = weightCubed - F64.C2 * weightSquared + interpolationAmount;
            Fix64 value2Blend = -2 * weightCubed + F64.C3 * weightSquared;
            Fix64 tangent2Blend = weightCubed - weightSquared;
            result.X = value1.X * value1Blend + value2.X * value2Blend + tangent1.X * tangent1Blend + tangent2.X * tangent2Blend;
            result.Y = value1.Y * value1Blend + value2.Y * value2Blend + tangent1.Y * tangent1Blend + tangent2.Y * tangent2Blend;
            result.Z = value1.Z * value1Blend + value2.Z * value2Blend + tangent1.Z * tangent1Blend + tangent2.Z * tangent2Blend;
        }
        /// <summary>
        /// Computes an intermediate location using hermite interpolation.
        /// </summary>
        /// <param name="value1">First position.</param>
        /// <param name="tangent1">Tangent associated with the first position.</param>
        /// <param name="value2">Second position.</param>
        /// <param name="tangent2">Tangent associated with the second position.</param>
        /// <param name="interpolationAmount">Amount of the second point to use.</param>
        /// <returns>Interpolated intermediate state.</returns>
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, Fix64 interpolationAmount)
        {
            Vector3 toReturn;
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, interpolationAmount, out toReturn);
            return toReturn;
        }
    }
}

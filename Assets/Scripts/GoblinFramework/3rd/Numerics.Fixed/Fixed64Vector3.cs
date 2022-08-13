#region License
/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/
#endregion

using System;

namespace Numerics.Fixed
{
    /// <summary>
    /// Represents a vector with three Q31.32 fixed-point values.
    /// </summary>
    [Serializable]
    public struct Fixed64Vector3
    {

        private static Fixed64 ZeroEpsilonSq = FixedMath.Epsilon;
        internal static Fixed64Vector3 InternalZero;
        internal static Fixed64Vector3 Arbitrary;

        /// <summary>The X component of the vector.</summary>
        public Fixed64 x;
        /// <summary>The Y component of the vector.</summary>
        public Fixed64 y;
        /// <summary>The Z component of the vector.</summary>
        public Fixed64 z;

        /// <summary>
        /// A vector with components (0,0,0);
        /// </summary>
        public static readonly Fixed64Vector3 zero;
        /// <summary>
        /// A vector with components (-1,0,0);
        /// </summary>
        public static readonly Fixed64Vector3 left;
        /// <summary>
        /// A vector with components (1,0,0);
        /// </summary>
        public static readonly Fixed64Vector3 right;
        /// <summary>
        /// A vector with components (0,1,0);
        /// </summary>
        public static readonly Fixed64Vector3 up;
        /// <summary>
        /// A vector with components (0,-1,0);
        /// </summary>
        public static readonly Fixed64Vector3 down;
        /// <summary>
        /// A vector with components (0,0,-1);
        /// </summary>
        public static readonly Fixed64Vector3 back;
        /// <summary>
        /// A vector with components (0,0,1);
        /// </summary>
        public static readonly Fixed64Vector3 forward;
        /// <summary>
        /// A vector with components (1,1,1);
        /// </summary>
        public static readonly Fixed64Vector3 one;
        /// <summary>
        /// A vector with components 
        /// (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly Fixed64Vector3 MinValue;
        /// <summary>
        /// A vector with components 
        /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly Fixed64Vector3 MaxValue;

        static Fixed64Vector3()
        {
            one = new Fixed64Vector3(1, 1, 1);
            zero = new Fixed64Vector3(0, 0, 0);
            left = new Fixed64Vector3(-1, 0, 0);
            right = new Fixed64Vector3(1, 0, 0);
            up = new Fixed64Vector3(0, 1, 0);
            down = new Fixed64Vector3(0, -1, 0);
            back = new Fixed64Vector3(0, 0, -1);
            forward = new Fixed64Vector3(0, 0, 1);
            MinValue = new Fixed64Vector3(Fixed64.MinValue);
            MaxValue = new Fixed64Vector3(Fixed64.MaxValue);
            Arbitrary = new Fixed64Vector3(1, 1, 1);
            InternalZero = zero;
        }

        public static Fixed64Vector3 Abs(Fixed64Vector3 other)
        {
            return new Fixed64Vector3(Fixed64.Abs(other.x), Fixed64.Abs(other.y), Fixed64.Abs(other.z));
        }

        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public Fixed64 SqrMagnitude
        {
            get
            {
                return (((x * x) + (y * y)) + (z * z));
            }
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        /// <returns>Returns the length of the vector.</returns>
        public Fixed64 Magnitude
        {
            get
            {
                Fixed64 num = ((x * x) + (y * y)) + (z * z);
                return Fixed64.Sqrt(num);
            }
        }

        public static Fixed64Vector3 ClampMagnitude(Fixed64Vector3 vector, Fixed64 maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// Gets a normalized version of the vector.
        /// </summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public Fixed64Vector3 Normalized
        {
            get
            {
                Fixed64Vector3 result = new Fixed64Vector3(x, y, z);
                result.Normalize();

                return result;
            }
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>

        public Fixed64Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Fixed64Vector3(Fixed64 x, Fixed64 y, Fixed64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public void Scale(Fixed64Vector3 other)
        {
            x = x * other.x;
            y = y * other.y;
            z = z * other.z;
        }

        /// <summary>
        /// Sets all vector component to specific values.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public void Set(Fixed64 x, Fixed64 y, Fixed64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="xyz">All components of the vector are set to xyz</param>
        public Fixed64Vector3(Fixed64 xyz)
        {
            x = xyz;
            y = xyz;
            z = xyz;
        }

        public static Fixed64Vector3 Lerp(Fixed64Vector3 from, Fixed64Vector3 to, Fixed64 percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        /// Builds a string from the JVector.
        /// </summary>
        /// <returns>A string containing all three components.</returns>
        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1})", x.AsFloat(), y.AsFloat(), z.AsFloat());
        }

        /// <summary>
        /// Tests if an object is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns true if they are euqal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Fixed64Vector3)) return false;
            Fixed64Vector3 other = (Fixed64Vector3)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public static Fixed64Vector3 Scale(Fixed64Vector3 vecA, Fixed64Vector3 vecB)
        {
            Fixed64Vector3 result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;

            return result;
        }

        /// <summary>
        /// Tests if two JVector are equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns true if both values are equal, otherwise false.</returns>
        public static bool operator ==(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z));
        }

        /// <summary>
        /// Tests if two JVector are not equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns false if both values are equal, otherwise true.</returns>
        public static bool operator !=(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            if ((value1.x == value2.x) && (value1.y == value2.y))
            {
                return (value1.z != value2.z);
            }
            return true;
        }

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        public static Fixed64Vector3 Min(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Min(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
        public static void Min(ref Fixed64Vector3 value1, ref Fixed64Vector3 value2, out Fixed64Vector3 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        public static Fixed64Vector3 Max(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Max(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        public static Fixed64 Distance(Fixed64Vector3 v1, Fixed64Vector3 v2)
        {
            return Fixed64.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
        public static void Max(ref Fixed64Vector3 value1, ref Fixed64Vector3 value2, out Fixed64Vector3 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
        }

        /// <summary>
        /// Sets the length of the vector to zero.
        /// </summary>
        public void MakeZero()
        {
            x = Fixed64.Zero;
            y = Fixed64.Zero;
            z = Fixed64.Zero;
        }

        /// <summary>
        /// Checks if the length of the vector is zero.
        /// </summary>
        /// <returns>Returns true if the vector is zero, otherwise false.</returns>
        public bool IsZero()
        {
            return (SqrMagnitude == Fixed64.Zero);
        }

        /// <summary>
        /// Checks if the length of the vector is nearly zero.
        /// </summary>
        /// <returns>Returns true if the vector is nearly zero, otherwise false.</returns>
        public bool IsNearlyZero()
        {
            return (SqrMagnitude < ZeroEpsilonSq);
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The transformed vector.</returns>
        public static Fixed64Vector3 Transform(Fixed64Vector3 position, Fixed64Matrix matrix)
        {
            Transform(ref position, ref matrix, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void Transform(ref Fixed64Vector3 position, ref Fixed64Matrix matrix, out Fixed64Vector3 result)
        {
            Fixed64 num0 = ((position.x * matrix.M11) + (position.y * matrix.M21)) + (position.z * matrix.M31);
            Fixed64 num1 = ((position.x * matrix.M12) + (position.y * matrix.M22)) + (position.z * matrix.M32);
            Fixed64 num2 = ((position.x * matrix.M13) + (position.y * matrix.M23)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Transforms a vector by the transposed of the given Matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void TransposedTransform(ref Fixed64Vector3 position, ref Fixed64Matrix matrix, out Fixed64Vector3 result)
        {
            Fixed64 num0 = ((position.x * matrix.M11) + (position.y * matrix.M12)) + (position.z * matrix.M13);
            Fixed64 num1 = ((position.x * matrix.M21) + (position.y * matrix.M22)) + (position.z * matrix.M23);
            Fixed64 num2 = ((position.x * matrix.M31) + (position.y * matrix.M32)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static Fixed64 Dot(Fixed64Vector3 vector1, Fixed64Vector3 vector2)
        {
            return Dot(ref vector1, ref vector2);
        }


        /// <summary>
        /// Calculates the dot product of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static Fixed64 Dot(ref Fixed64Vector3 vector1, ref Fixed64Vector3 vector2)
        {
            return ((vector1.x * vector2.x) + (vector1.y * vector2.y)) + (vector1.z * vector2.z);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        public static Fixed64Vector3 Add(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Add(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Adds to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The sum of both vectors.</param>
        public static void Add(ref Fixed64Vector3 value1, ref Fixed64Vector3 value2, out Fixed64Vector3 result)
        {
            Fixed64 num0 = value1.x + value2.x;
            Fixed64 num1 = value1.y + value2.y;
            Fixed64 num2 = value1.z + value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static Fixed64Vector3 Divide(Fixed64Vector3 value1, Fixed64 scaleFactor)
        {
            Divide(ref value1, scaleFactor, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the scaled vector.</param>
        public static void Divide(ref Fixed64Vector3 value1, Fixed64 scaleFactor, out Fixed64Vector3 result)
        {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        public static Fixed64Vector3 Subtract(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Subtract(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Subtracts to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The difference of both vectors.</param>
        public static void Subtract(ref Fixed64Vector3 value1, ref Fixed64Vector3 value2, out Fixed64Vector3 result)
        {
            Fixed64 num0 = value1.x - value2.x;
            Fixed64 num1 = value1.y - value2.y;
            Fixed64 num2 = value1.z - value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        public static Fixed64Vector3 Cross(Fixed64Vector3 vector1, Fixed64Vector3 vector2)
        {
            Cross(ref vector1, ref vector2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of both vectors.</param>
        public static void Cross(ref Fixed64Vector3 vector1, ref Fixed64Vector3 vector2, out Fixed64Vector3 result)
        {
            Fixed64 num3 = (vector1.y * vector2.z) - (vector1.z * vector2.y);
            Fixed64 num2 = (vector1.z * vector2.x) - (vector1.x * vector2.z);
            Fixed64 num = (vector1.x * vector2.y) - (vector1.y * vector2.x);
            result.x = num3;
            result.y = num2;
            result.z = num;
        }

        /// <summary>
        /// Gets the hashcode of the vector.
        /// </summary>
        /// <returns>Returns the hashcode of the vector.</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        /// <summary>
        /// Inverses the direction of the vector.
        /// </summary>
        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <returns>The negated vector.</returns>
        public static Fixed64Vector3 Negate(Fixed64Vector3 value)
        {
            Negate(ref value, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <param name="result">The negated vector.</param>
        public static void Negate(ref Fixed64Vector3 value, out Fixed64Vector3 result)
        {
            Fixed64 num0 = -value.x;
            Fixed64 num1 = -value.y;
            Fixed64 num2 = -value.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        public static Fixed64Vector3 Normalize(Fixed64Vector3 value)
        {
            Normalize(ref value, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            Fixed64 num2 = ((x * x) + (y * y)) + (z * z);
            Fixed64 num = Fixed64.One / Fixed64.Sqrt(num2);
            x *= num;
            y *= num;
            z *= num;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="result">A normalized vector.</param>
        public static void Normalize(ref Fixed64Vector3 value, out Fixed64Vector3 result)
        {
            Fixed64 num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z);
            Fixed64 num = Fixed64.One / Fixed64.Sqrt(num2);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }

        /// <summary>
        /// Swaps the components of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector to swap with the second.</param>
        /// <param name="vector2">The second vector to swap with the first.</param>
        public static void Swap(ref Fixed64Vector3 vector1, ref Fixed64Vector3 vector2)
        {
            Fixed64 temp;

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        public static Fixed64Vector3 Multiply(Fixed64Vector3 value1, Fixed64 scaleFactor)
        {
            Multiply(ref value1, scaleFactor, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the multiplied vector.</param>
        public static void Multiply(ref Fixed64Vector3 value1, Fixed64 scaleFactor, out Fixed64Vector3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the cross product of both.</returns>
        public static Fixed64Vector3 operator %(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Cross(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the dot product of both.</returns>
        public static Fixed64 operator *(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            return Dot(ref value1, ref value2);
        }

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value1">The vector to scale.</param>
        /// <param name="value2">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static Fixed64Vector3 operator *(Fixed64Vector3 value1, Fixed64 value2)
        {
            Multiply(ref value1, value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value2">The vector to scale.</param>
        /// <param name="value1">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static Fixed64Vector3 operator *(Fixed64 value1, Fixed64Vector3 value2)
        {
            Multiply(ref value2, value1, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        public static Fixed64Vector3 operator -(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Subtract(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        public static Fixed64Vector3 operator +(Fixed64Vector3 value1, Fixed64Vector3 value2)
        {
            Add(ref value1, ref value2, out Fixed64Vector3 result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static Fixed64Vector3 operator /(Fixed64Vector3 value1, Fixed64 value2)
        {
            Divide(ref value1, value2, out Fixed64Vector3 result);
            return result;
        }

        public static Fixed64 Angle(Fixed64Vector3 a, Fixed64Vector3 b)
        {
            return Fixed64.Acos(a.Normalized * b.Normalized) * Fixed64.Rad2Deg;
        }

        public Fixed64Vector2 ToTSVector2()
        {
            return new Fixed64Vector2(x, y);
        }

    }

}

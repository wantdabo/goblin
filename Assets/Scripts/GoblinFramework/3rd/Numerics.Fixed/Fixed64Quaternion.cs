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

namespace Numerics.Fixed
{
    using System;

    /// <summary>
    /// Represents a vector that is used to encode three-dimensional physical rotations.
    /// </summary>
    [Serializable]
    public struct Fixed64Quaternion
    {

        /// <summary>The X component of the quaternion.</summary>
        public Fixed64 x;
        /// <summary>The Y component of the quaternion.</summary>
        public Fixed64 y;
        /// <summary>The Z component of the quaternion.</summary>
        public Fixed64 z;
        /// <summary>The W component of the quaternion.</summary>
        public Fixed64 w;

        public static readonly Fixed64Quaternion identity;

        static Fixed64Quaternion()
        {
            identity = new Fixed64Quaternion(0, 0, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the JQuaternion structure.
        /// </summary>
        /// <param name="x">The X component of the quaternion.</param>
        /// <param name="y">The Y component of the quaternion.</param>
        /// <param name="z">The Z component of the quaternion.</param>
        /// <param name="w">The W component of the quaternion.</param>
        public Fixed64Quaternion(Fixed64 x, Fixed64 y, Fixed64 z, Fixed64 w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Set(Fixed64 new_x, Fixed64 new_y, Fixed64 new_z, Fixed64 new_w)
        {
            x = new_x;
            y = new_y;
            z = new_z;
            w = new_w;
        }

        public void SetFromToRotation(Fixed64Vector3 fromDirection, Fixed64Vector3 toDirection)
        {
            Fixed64Quaternion targetRotation = FromToRotation(fromDirection, toDirection);
            Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
        }

        public Fixed64Vector3 EulerAngles
        {
            get
            {
                Fixed64Vector3 result = new Fixed64Vector3();

                Fixed64 ysqr = y * y;
                Fixed64 t0 = -2.0f * (ysqr + z * z) + 1.0f;
                Fixed64 t1 = +2.0f * (x * y - w * z);
                Fixed64 t2 = -2.0f * (x * z + w * y);
                Fixed64 t3 = +2.0f * (y * z - w * x);
                Fixed64 t4 = -2.0f * (x * x + ysqr) + 1.0f;

                t2 = t2 > 1.0f ? 1.0f : t2;
                t2 = t2 < -1.0f ? -1.0f : t2;

                result.x = Fixed64.Atan2(t3, t4) * Fixed64.Rad2Deg;
                result.y = Fixed64.Asin(t2) * Fixed64.Rad2Deg;
                result.z = Fixed64.Atan2(t1, t0) * Fixed64.Rad2Deg;

                return result * -1;
            }
        }

        public static Fixed64 Angle(Fixed64Quaternion a, Fixed64Quaternion b)
        {
            Fixed64Quaternion aInv = Inverse(a);
            Fixed64Quaternion f = b * aInv;

            Fixed64 angle = Fixed64.Acos(f.w) * 2 * Fixed64.Rad2Deg;

            if (angle > 180)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
        public static Fixed64Quaternion Add(Fixed64Quaternion quaternion1, Fixed64Quaternion quaternion2)
        {
            Add(ref quaternion1, ref quaternion2, out Fixed64Quaternion result);
            return result;
        }

        public static Fixed64Quaternion LookRotation(Fixed64Vector3 forward)
        {
            return CreateFromMatrix(Fixed64Matrix.LookAt(forward, Fixed64Vector3.up));
        }

        public static Fixed64Quaternion LookRotation(Fixed64Vector3 forward, Fixed64Vector3 upwards)
        {
            return CreateFromMatrix(Fixed64Matrix.LookAt(forward, upwards));
        }

        public static Fixed64Quaternion Slerp(Fixed64Quaternion from, Fixed64Quaternion to, Fixed64 t)
        {
            t = FixedMath.Clamp(t, 0, 1);

            Fixed64 dot = Dot(from, to);

            if (dot < 0.0f)
            {
                to = Multiply(to, -1);
                dot = -dot;
            }

            Fixed64 halfTheta = Fixed64.Acos(dot);

            return Multiply(Multiply(from, Fixed64.Sin((1 - t) * halfTheta)) + Multiply(to, Fixed64.Sin(t * halfTheta)), 1 / Fixed64.Sin(halfTheta));
        }

        public static Fixed64Quaternion RotateTowards(Fixed64Quaternion from, Fixed64Quaternion to, Fixed64 maxDegreesDelta)
        {
            Fixed64 dot = Dot(from, to);

            if (dot < 0.0f)
            {
                to = Multiply(to, -1);
                dot = -dot;
            }

            Fixed64 halfTheta = Fixed64.Acos(dot);
            Fixed64 theta = halfTheta * 2;

            maxDegreesDelta *= Fixed64.Deg2Rad;

            if (maxDegreesDelta >= theta)
            {
                return to;
            }

            maxDegreesDelta /= theta;

            return Multiply(Multiply(from, Fixed64.Sin((1 - maxDegreesDelta) * halfTheta)) + Multiply(to, Fixed64.Sin(maxDegreesDelta * halfTheta)), 1 / Fixed64.Sin(halfTheta));
        }

        public static Fixed64Quaternion Euler(Fixed64 x, Fixed64 y, Fixed64 z)
        {
            x *= Fixed64.Deg2Rad;
            y *= Fixed64.Deg2Rad;
            z *= Fixed64.Deg2Rad;

            CreateFromYawPitchRoll(y, x, z, out Fixed64Quaternion rotation);

            return rotation;
        }

        public static Fixed64Quaternion Euler(Fixed64Vector3 eulerAngles)
        {
            return Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public static Fixed64Quaternion AngleAxis(Fixed64 angle, Fixed64Vector3 axis)
        {
            axis = axis * Fixed64.Deg2Rad;
            axis.Normalize();

            Fixed64 halfAngle = angle * Fixed64.Deg2Rad * Fixed64.Half;

            Fixed64Quaternion rotation;
            Fixed64 sin = Fixed64.Sin(halfAngle);

            rotation.x = axis.x * sin;
            rotation.y = axis.y * sin;
            rotation.z = axis.z * sin;
            rotation.w = Fixed64.Cos(halfAngle);

            return rotation;
        }

        public static void CreateFromYawPitchRoll(Fixed64 yaw, Fixed64 pitch, Fixed64 roll, out Fixed64Quaternion result)
        {
            Fixed64 num9 = roll * Fixed64.Half;
            Fixed64 num6 = Fixed64.Sin(num9);
            Fixed64 num5 = Fixed64.Cos(num9);
            Fixed64 num8 = pitch * Fixed64.Half;
            Fixed64 num4 = Fixed64.Sin(num8);
            Fixed64 num3 = Fixed64.Cos(num8);
            Fixed64 num7 = yaw * Fixed64.Half;
            Fixed64 num2 = Fixed64.Sin(num7);
            Fixed64 num = Fixed64.Cos(num7);
            result.x = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.w = ((num * num3) * num5) + ((num2 * num4) * num6);
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The sum of both quaternions.</param>
        public static void Add(ref Fixed64Quaternion quaternion1, ref Fixed64Quaternion quaternion2, out Fixed64Quaternion result)
        {
            result.x = quaternion1.x + quaternion2.x;
            result.y = quaternion1.y + quaternion2.y;
            result.z = quaternion1.z + quaternion2.z;
            result.w = quaternion1.w + quaternion2.w;
        }
        #endregion

        public static Fixed64Quaternion Conjugate(Fixed64Quaternion value)
        {
            Fixed64Quaternion quaternion;
            quaternion.x = -value.x;
            quaternion.y = -value.y;
            quaternion.z = -value.z;
            quaternion.w = value.w;
            return quaternion;
        }

        public static Fixed64 Dot(Fixed64Quaternion a, Fixed64Quaternion b)
        {
            return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Fixed64Quaternion Inverse(Fixed64Quaternion rotation)
        {
            Fixed64 invNorm = Fixed64.One / ((rotation.x * rotation.x) + (rotation.y * rotation.y) + (rotation.z * rotation.z) + (rotation.w * rotation.w));
            return Multiply(Conjugate(rotation), invNorm);
        }

        public static Fixed64Quaternion FromToRotation(Fixed64Vector3 fromVector, Fixed64Vector3 toVector)
        {
            Fixed64Vector3 w = Fixed64Vector3.Cross(fromVector, toVector);
            Fixed64Quaternion q = new Fixed64Quaternion(w.x, w.y, w.z, Fixed64Vector3.Dot(fromVector, toVector));
            q.w += Fixed64.Sqrt(fromVector.SqrMagnitude * toVector.SqrMagnitude);
            q.Normalize();

            return q;
        }

        public static Fixed64Quaternion Lerp(Fixed64Quaternion a, Fixed64Quaternion b, Fixed64 t)
        {
            t = FixedMath.Clamp(t, Fixed64.Zero, Fixed64.One);

            return LerpUnclamped(a, b, t);
        }

        public static Fixed64Quaternion LerpUnclamped(Fixed64Quaternion a, Fixed64Quaternion b, Fixed64 t)
        {
            Fixed64Quaternion result = Multiply(a, (1 - t)) + Multiply(b, t);
            result.Normalize();

            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)
        public static Fixed64Quaternion Subtract(Fixed64Quaternion quaternion1, Fixed64Quaternion quaternion2)
        {
            Subtract(ref quaternion1, ref quaternion2, out Fixed64Quaternion result);
            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The difference of both quaternions.</param>
        public static void Subtract(ref Fixed64Quaternion quaternion1, ref Fixed64Quaternion quaternion2, out Fixed64Quaternion result)
        {
            result.x = quaternion1.x - quaternion2.x;
            result.y = quaternion1.y - quaternion2.y;
            result.z = quaternion1.z - quaternion2.z;
            result.w = quaternion1.w - quaternion2.w;
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)
        public static Fixed64Quaternion Multiply(Fixed64Quaternion quaternion1, Fixed64Quaternion quaternion2)
        {
            Multiply(ref quaternion1, ref quaternion2, out Fixed64Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The product of both quaternions.</param>
        public static void Multiply(ref Fixed64Quaternion quaternion1, ref Fixed64Quaternion quaternion2, out Fixed64Quaternion result)
        {
            Fixed64 x = quaternion1.x;
            Fixed64 y = quaternion1.y;
            Fixed64 z = quaternion1.z;
            Fixed64 w = quaternion1.w;
            Fixed64 num4 = quaternion2.x;
            Fixed64 num3 = quaternion2.y;
            Fixed64 num2 = quaternion2.z;
            Fixed64 num = quaternion2.w;
            Fixed64 num12 = (y * num2) - (z * num3);
            Fixed64 num11 = (z * num4) - (x * num2);
            Fixed64 num10 = (x * num3) - (y * num4);
            Fixed64 num9 = ((x * num4) + (y * num3)) + (z * num2);
            result.x = ((x * num) + (num4 * w)) + num12;
            result.y = ((y * num) + (num3 * w)) + num11;
            result.z = ((z * num) + (num2 * w)) + num10;
            result.w = (w * num) - num9;
        }
        #endregion

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>The scaled quaternion.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, FP scaleFactor)
        public static Fixed64Quaternion Multiply(Fixed64Quaternion quaternion1, Fixed64 scaleFactor)
        {
            Multiply(ref quaternion1, scaleFactor, out Fixed64Quaternion result);
            return result;
        }

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <param name="result">The scaled quaternion.</param>
        public static void Multiply(ref Fixed64Quaternion quaternion1, Fixed64 scaleFactor, out Fixed64Quaternion result)
        {
            result.x = quaternion1.x * scaleFactor;
            result.y = quaternion1.y * scaleFactor;
            result.z = quaternion1.z * scaleFactor;
            result.w = quaternion1.w * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Sets the length of the quaternion to one.
        /// </summary>
        #region public void Normalize()
        public void Normalize()
        {
            Fixed64 num2 = (((x * x) + (y * y)) + (z * z)) + (w * w);
            Fixed64 num = 1 / (Fixed64.Sqrt(num2));
            x *= num;
            y *= num;
            z *= num;
            w *= num;
        }
        #endregion

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <returns>JQuaternion representing an orientation.</returns>
        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)
        public static Fixed64Quaternion CreateFromMatrix(Fixed64Matrix matrix)
        {
            CreateFromMatrix(ref matrix, out Fixed64Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <param name="result">JQuaternion representing an orientation.</param>
        public static void CreateFromMatrix(ref Fixed64Matrix matrix, out Fixed64Quaternion result)
        {
            Fixed64 num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > Fixed64.Zero)
            {
                Fixed64 num = Fixed64.Sqrt((num8 + Fixed64.One));
                result.w = num * Fixed64.Half;
                num = Fixed64.Half / num;
                result.x = (matrix.M23 - matrix.M32) * num;
                result.y = (matrix.M31 - matrix.M13) * num;
                result.z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                Fixed64 num7 = Fixed64.Sqrt((((Fixed64.One + matrix.M11) - matrix.M22) - matrix.M33));
                Fixed64 num4 = Fixed64.Half / num7;
                result.x = Fixed64.Half * num7;
                result.y = (matrix.M12 + matrix.M21) * num4;
                result.z = (matrix.M13 + matrix.M31) * num4;
                result.w = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                Fixed64 num6 = Fixed64.Sqrt((((Fixed64.One + matrix.M22) - matrix.M11) - matrix.M33));
                Fixed64 num3 = Fixed64.Half / num6;
                result.x = (matrix.M21 + matrix.M12) * num3;
                result.y = Fixed64.Half * num6;
                result.z = (matrix.M32 + matrix.M23) * num3;
                result.w = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                Fixed64 num5 = Fixed64.Sqrt((((Fixed64.One + matrix.M33) - matrix.M11) - matrix.M22));
                Fixed64 num2 = Fixed64.Half / num5;
                result.x = (matrix.M31 + matrix.M13) * num2;
                result.y = (matrix.M32 + matrix.M23) * num2;
                result.z = Fixed64.Half * num5;
                result.w = (matrix.M12 - matrix.M21) * num2;
            }
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static FP operator *(JQuaternion value1, JQuaternion value2)
        public static Fixed64Quaternion operator *(Fixed64Quaternion value1, Fixed64Quaternion value2)
        {
            Multiply(ref value1, ref value2, out Fixed64Quaternion result);
            return result;
        }
        #endregion

        /// <summary>
        /// Add two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static FP operator +(JQuaternion value1, JQuaternion value2)
        public static Fixed64Quaternion operator +(Fixed64Quaternion value1, Fixed64Quaternion value2)
        {
            Add(ref value1, ref value2, out Fixed64Quaternion result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtract two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static FP operator -(JQuaternion value1, JQuaternion value2)
        public static Fixed64Quaternion operator -(Fixed64Quaternion value1, Fixed64Quaternion value2)
        {
            Subtract(ref value1, ref value2, out Fixed64Quaternion result);
            return result;
        }
        #endregion

        /**
         *  @brief Rotates a {@link TSVector} by the {@link TSQuanternion}.
         **/
        public static Fixed64Vector3 operator *(Fixed64Quaternion quat, Fixed64Vector3 vec)
        {
            Fixed64 num = quat.x * 2f;
            Fixed64 num2 = quat.y * 2f;
            Fixed64 num3 = quat.z * 2f;
            Fixed64 num4 = quat.x * num;
            Fixed64 num5 = quat.y * num2;
            Fixed64 num6 = quat.z * num3;
            Fixed64 num7 = quat.x * num2;
            Fixed64 num8 = quat.x * num3;
            Fixed64 num9 = quat.y * num3;
            Fixed64 num10 = quat.w * num;
            Fixed64 num11 = quat.w * num2;
            Fixed64 num12 = quat.w * num3;

            Fixed64Vector3 result;
            result.x = (1f - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1f - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1f - (num4 + num5)) * vec.z;

            return result;
        }

        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }
    }
}

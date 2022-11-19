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
    /// <summary>
    /// Represents a 3x3 matrix.
    /// </summary>
    public struct Fixed64Matrix
    {
        /// <summary>
        /// M11
        /// </summary>
        public Fixed64 M11; // 1st row vector
        /// <summary>
        /// M12
        /// </summary>
        public Fixed64 M12;
        /// <summary>
        /// M13
        /// </summary>
        public Fixed64 M13;
        /// <summary>
        /// M21
        /// </summary>
        public Fixed64 M21; // 2nd row vector
        /// <summary>
        /// M22
        /// </summary>
        public Fixed64 M22;
        /// <summary>
        /// M23
        /// </summary>
        public Fixed64 M23;
        /// <summary>
        /// M31
        /// </summary>
        public Fixed64 M31; // 3rd row vector
        /// <summary>
        /// M32
        /// </summary>
        public Fixed64 M32;
        /// <summary>
        /// M33
        /// </summary>
        public Fixed64 M33;

        internal static Fixed64Matrix InternalIdentity;

        /// <summary>
        /// Identity matrix.
        /// </summary>
        public static readonly Fixed64Matrix Identity;
        public static readonly Fixed64Matrix Zero;

        static Fixed64Matrix()
        {
            Zero = new Fixed64Matrix();

            Identity = new Fixed64Matrix
            {
                M11 = Fixed64.One,
                M22 = Fixed64.One,
                M33 = Fixed64.One
            };

            InternalIdentity = Identity;
        }

        public Fixed64Vector3 EulerAngles
        {
            get
            {
                Fixed64Vector3 result = new Fixed64Vector3
                {
                    x = FixedMath.Atan2(M32, M33) * Fixed64.Rad2Deg,
                    y = FixedMath.Atan2(-M31, FixedMath.Sqrt(M32 * M32 + M33 * M33)) * Fixed64.Rad2Deg,
                    z = FixedMath.Atan2(M21, M11) * Fixed64.Rad2Deg
                };

                return result * -1;
            }
        }

        public static Fixed64Matrix CreateFromYawPitchRoll(Fixed64 yaw, Fixed64 pitch, Fixed64 roll)
        {
            Fixed64Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out Fixed64Quaternion quaternion);
            CreateFromQuaternion(ref quaternion, out Fixed64Matrix matrix);
            return matrix;
        }

        public static Fixed64Matrix CreateRotationX(Fixed64 radians)
        {
            Fixed64Matrix matrix;
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            matrix.M11 = Fixed64.One;
            matrix.M12 = Fixed64.Zero;
            matrix.M13 = Fixed64.Zero;
            matrix.M21 = Fixed64.Zero;
            matrix.M22 = num2;
            matrix.M23 = num;
            matrix.M31 = Fixed64.Zero;
            matrix.M32 = -num;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationX(Fixed64 radians, out Fixed64Matrix result)
        {
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            result.M11 = Fixed64.One;
            result.M12 = Fixed64.Zero;
            result.M13 = Fixed64.Zero;
            result.M21 = Fixed64.Zero;
            result.M22 = num2;
            result.M23 = num;
            result.M31 = Fixed64.Zero;
            result.M32 = -num;
            result.M33 = num2;
        }

        public static Fixed64Matrix CreateRotationY(Fixed64 radians)
        {
            Fixed64Matrix matrix;
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = Fixed64.Zero;
            matrix.M13 = -num;
            matrix.M21 = Fixed64.Zero;
            matrix.M22 = Fixed64.One;
            matrix.M23 = Fixed64.Zero;
            matrix.M31 = num;
            matrix.M32 = Fixed64.Zero;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationY(Fixed64 radians, out Fixed64Matrix result)
        {
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            result.M11 = num2;
            result.M12 = Fixed64.Zero;
            result.M13 = -num;
            result.M21 = Fixed64.Zero;
            result.M22 = Fixed64.One;
            result.M23 = Fixed64.Zero;
            result.M31 = num;
            result.M32 = Fixed64.Zero;
            result.M33 = num2;
        }

        public static Fixed64Matrix CreateRotationZ(Fixed64 radians)
        {
            Fixed64Matrix matrix;
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = Fixed64.Zero;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = Fixed64.Zero;
            matrix.M31 = Fixed64.Zero;
            matrix.M32 = Fixed64.Zero;
            matrix.M33 = Fixed64.One;
            return matrix;
        }


        public static void CreateRotationZ(Fixed64 radians, out Fixed64Matrix result)
        {
            Fixed64 num2 = Fixed64.Cos(radians);
            Fixed64 num = Fixed64.Sin(radians);
            result.M11 = num2;
            result.M12 = num;
            result.M13 = Fixed64.Zero;
            result.M21 = -num;
            result.M22 = num2;
            result.M23 = Fixed64.Zero;
            result.M31 = Fixed64.Zero;
            result.M32 = Fixed64.Zero;
            result.M33 = Fixed64.One;
        }

        /// <summary>
        /// Initializes a new instance of the matrix structure.
        /// </summary>
        /// <param name="m11">m11</param>
        /// <param name="m12">m12</param>
        /// <param name="m13">m13</param>
        /// <param name="m21">m21</param>
        /// <param name="m22">m22</param>
        /// <param name="m23">m23</param>
        /// <param name="m31">m31</param>
        /// <param name="m32">m32</param>
        /// <param name="m33">m33</param>
        public Fixed64Matrix(Fixed64 m11, Fixed64 m12, Fixed64 m13, Fixed64 m21, Fixed64 m22, Fixed64 m23, Fixed64 m31, Fixed64 m32, Fixed64 m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        /// Gets the determinant of the matrix.
        /// </summary>
        /// <returns>The determinant of the matrix.</returns>
        public Fixed64 Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
            //return M11 * M22 * M33 - M11 * M23 * M32 - M12 * M21 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31;
        }

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The product of both matrices.</returns>
        public static Fixed64Matrix Multiply(Fixed64Matrix matrix1, Fixed64Matrix matrix2)
        {
            Multiply(ref matrix1, ref matrix2, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The product of both matrices.</param>
        public static void Multiply(ref Fixed64Matrix matrix1, ref Fixed64Matrix matrix2, out Fixed64Matrix result)
        {
            Fixed64 num0 = ((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31);
            Fixed64 num1 = ((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32);
            Fixed64 num2 = ((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33);
            Fixed64 num3 = ((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31);
            Fixed64 num4 = ((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32);
            Fixed64 num5 = ((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33);
            Fixed64 num6 = ((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31);
            Fixed64 num7 = ((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32);
            Fixed64 num8 = ((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33);

            result.M11 = num0;
            result.M12 = num1;
            result.M13 = num2;
            result.M21 = num3;
            result.M22 = num4;
            result.M23 = num5;
            result.M31 = num6;
            result.M32 = num7;
            result.M33 = num8;
        }

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The sum of both matrices.</returns>
        public static Fixed64Matrix Add(Fixed64Matrix matrix1, Fixed64Matrix matrix2)
        {
            Add(ref matrix1, ref matrix2, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The sum of both matrices.</param>
        public static void Add(ref Fixed64Matrix matrix1, ref Fixed64Matrix matrix2, out Fixed64Matrix result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }

        /// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverted JMatrix.</returns>
        public static Fixed64Matrix Inverse(Fixed64Matrix matrix)
        {
            Inverse(ref matrix, out Fixed64Matrix result);
            return result;
        }

        public static void Invert(ref Fixed64Matrix matrix, out Fixed64Matrix result)
        {
            Fixed64 determinantInverse = 1 / matrix.Determinant();
            Fixed64 m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
            Fixed64 m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
            Fixed64 m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

            Fixed64 m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
            Fixed64 m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
            Fixed64 m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

            Fixed64 m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
            Fixed64 m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
            Fixed64 m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <param name="result">The inverted JMatrix.</param>
        public static void Inverse(ref Fixed64Matrix matrix, out Fixed64Matrix result)
        {
            Fixed64 det = 1024 * matrix.M11 * matrix.M22 * matrix.M33 -
                1024 * matrix.M11 * matrix.M23 * matrix.M32 -
                1024 * matrix.M12 * matrix.M21 * matrix.M33 +
                1024 * matrix.M12 * matrix.M23 * matrix.M31 +
                1024 * matrix.M13 * matrix.M21 * matrix.M32 -
                1024 * matrix.M13 * matrix.M22 * matrix.M31;

            Fixed64 num11 = 1024 * matrix.M22 * matrix.M33 - 1024 * matrix.M23 * matrix.M32;
            Fixed64 num12 = 1024 * matrix.M13 * matrix.M32 - 1024 * matrix.M12 * matrix.M33;
            Fixed64 num13 = 1024 * matrix.M12 * matrix.M23 - 1024 * matrix.M22 * matrix.M13;

            Fixed64 num21 = 1024 * matrix.M23 * matrix.M31 - 1024 * matrix.M33 * matrix.M21;
            Fixed64 num22 = 1024 * matrix.M11 * matrix.M33 - 1024 * matrix.M31 * matrix.M13;
            Fixed64 num23 = 1024 * matrix.M13 * matrix.M21 - 1024 * matrix.M23 * matrix.M11;

            Fixed64 num31 = 1024 * matrix.M21 * matrix.M32 - 1024 * matrix.M31 * matrix.M22;
            Fixed64 num32 = 1024 * matrix.M12 * matrix.M31 - 1024 * matrix.M32 * matrix.M11;
            Fixed64 num33 = 1024 * matrix.M11 * matrix.M22 - 1024 * matrix.M21 * matrix.M12;

            if (det == 0)
            {
                result.M11 = Fixed64.PositiveInfinity;
                result.M12 = Fixed64.PositiveInfinity;
                result.M13 = Fixed64.PositiveInfinity;
                result.M21 = Fixed64.PositiveInfinity;
                result.M22 = Fixed64.PositiveInfinity;
                result.M23 = Fixed64.PositiveInfinity;
                result.M31 = Fixed64.PositiveInfinity;
                result.M32 = Fixed64.PositiveInfinity;
                result.M33 = Fixed64.PositiveInfinity;
            }
            else
            {
                result.M11 = num11 / det;
                result.M12 = num12 / det;
                result.M13 = num13 / det;
                result.M21 = num21 / det;
                result.M22 = num22 / det;
                result.M23 = num23 / det;
                result.M31 = num31 / det;
                result.M32 = num32 / det;
                result.M33 = num33 / det;
            }

        }

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>A JMatrix multiplied by the scale factor.</returns>
        public static Fixed64Matrix Multiply(Fixed64Matrix matrix1, Fixed64 scaleFactor)
        {
            Multiply(ref matrix1, scaleFactor, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">A JMatrix multiplied by the scale factor.</param>
        public static void Multiply(ref Fixed64Matrix matrix1, Fixed64 scaleFactor, out Fixed64Matrix result)
        {
            Fixed64 num = scaleFactor;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
        }

        /// <summary>
        /// Creates a JMatrix representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <returns>JMatrix representing an orientation.</returns>
        public static Fixed64Matrix CreateFromLookAt(Fixed64Vector3 position, Fixed64Vector3 target)
        {
            LookAt(target - position, Fixed64Vector3.up, out Fixed64Matrix result);
            return result;
        }

        public static Fixed64Matrix LookAt(Fixed64Vector3 forward, Fixed64Vector3 upwards)
        {
            LookAt(forward, upwards, out Fixed64Matrix result);

            return result;
        }

        public static void LookAt(Fixed64Vector3 forward, Fixed64Vector3 upwards, out Fixed64Matrix result)
        {
            Fixed64Vector3 zaxis = forward; zaxis.Normalize();
            Fixed64Vector3 xaxis = Fixed64Vector3.Cross(upwards, zaxis); xaxis.Normalize();
            Fixed64Vector3 yaxis = Fixed64Vector3.Cross(zaxis, xaxis);

            result.M11 = xaxis.x;
            result.M21 = yaxis.x;
            result.M31 = zaxis.x;
            result.M12 = xaxis.y;
            result.M22 = yaxis.y;
            result.M32 = zaxis.y;
            result.M13 = xaxis.z;
            result.M23 = yaxis.z;
            result.M33 = zaxis.z;
        }

        public static Fixed64Matrix CreateFromQuaternion(Fixed64Quaternion quaternion)
        {
            CreateFromQuaternion(ref quaternion, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a JMatrix representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <param name="result">JMatrix representing an orientation.</param>
        public static void CreateFromQuaternion(ref Fixed64Quaternion quaternion, out Fixed64Matrix result)
        {
            Fixed64 num9 = quaternion.x * quaternion.x;
            Fixed64 num8 = quaternion.y * quaternion.y;
            Fixed64 num7 = quaternion.z * quaternion.z;
            Fixed64 num6 = quaternion.x * quaternion.y;
            Fixed64 num5 = quaternion.z * quaternion.w;
            Fixed64 num4 = quaternion.z * quaternion.x;
            Fixed64 num3 = quaternion.y * quaternion.w;
            Fixed64 num2 = quaternion.y * quaternion.z;
            Fixed64 num = quaternion.x * quaternion.w;
            result.M11 = Fixed64.One - (2 * (num8 + num7));
            result.M12 = 2 * (num6 + num5);
            result.M13 = 2 * (num4 - num3);
            result.M21 = 2 * (num6 - num5);
            result.M22 = Fixed64.One - (2 * (num7 + num9));
            result.M23 = 2 * (num2 + num);
            result.M31 = 2 * (num4 + num3);
            result.M32 = 2 * (num2 - num);
            result.M33 = Fixed64.One - (2 * (num8 + num9));
        }

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <returns>The transposed JMatrix.</returns>
        public static Fixed64Matrix Transpose(Fixed64Matrix matrix)
        {
            Transpose(ref matrix, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <param name="result">The transposed JMatrix.</param>
        public static void Transpose(ref Fixed64Matrix matrix, out Fixed64Matrix result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The product of both values.</returns>
        public static Fixed64Matrix operator *(Fixed64Matrix value1, Fixed64Matrix value2)
        {
            Multiply(ref value1, ref value2, out Fixed64Matrix result);
            return result;
        }


        public Fixed64 Trace()
        {
            return M11 + M22 + M33;
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The sum of both values.</returns>
        public static Fixed64Matrix operator +(Fixed64Matrix value1, Fixed64Matrix value2)
        {
            Add(ref value1, ref value2, out Fixed64Matrix result);
            return result;
        }

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The difference of both values.</returns>
        public static Fixed64Matrix operator -(Fixed64Matrix value1, Fixed64Matrix value2)
        {
            Multiply(ref value2, -Fixed64.One, out value2);
            Add(ref value1, ref value2, out Fixed64Matrix result);
            return result;
        }

        public static bool operator ==(Fixed64Matrix value1, Fixed64Matrix value2)
        {
            return value1.M11 == value2.M11 &&
                value1.M12 == value2.M12 &&
                value1.M13 == value2.M13 &&
                value1.M21 == value2.M21 &&
                value1.M22 == value2.M22 &&
                value1.M23 == value2.M23 &&
                value1.M31 == value2.M31 &&
                value1.M32 == value2.M32 &&
                value1.M33 == value2.M33;
        }

        public static bool operator !=(Fixed64Matrix value1, Fixed64Matrix value2)
        {
            return value1.M11 != value2.M11 ||
                value1.M12 != value2.M12 ||
                value1.M13 != value2.M13 ||
                value1.M21 != value2.M21 ||
                value1.M22 != value2.M22 ||
                value1.M23 != value2.M23 ||
                value1.M31 != value2.M31 ||
                value1.M32 != value2.M32 ||
                value1.M33 != value2.M33;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Fixed64Matrix)) return false;
            Fixed64Matrix other = (Fixed64Matrix)obj;

            return M11 == other.M11 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M21 == other.M21 &&
                M22 == other.M22 &&
                M23 == other.M23 &&
                M31 == other.M31 &&
                M32 == other.M32 &&
                M33 == other.M33;
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^
                M12.GetHashCode() ^
                M13.GetHashCode() ^
                M21.GetHashCode() ^
                M22.GetHashCode() ^
                M23.GetHashCode() ^
                M31.GetHashCode() ^
                M32.GetHashCode() ^
                M33.GetHashCode();
        }

        /// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="result">The resulting rotation matrix</param>
        public static void CreateFromAxisAngle(ref Fixed64Vector3 axis, Fixed64 angle, out Fixed64Matrix result)
        {
            Fixed64 x = axis.x;
            Fixed64 y = axis.y;
            Fixed64 z = axis.z;
            Fixed64 num2 = Fixed64.Sin(angle);
            Fixed64 num = Fixed64.Cos(angle);
            Fixed64 num11 = x * x;
            Fixed64 num10 = y * y;
            Fixed64 num9 = z * z;
            Fixed64 num8 = x * y;
            Fixed64 num7 = x * z;
            Fixed64 num6 = y * z;
            result.M11 = num11 + (num * (Fixed64.One - num11));
            result.M12 = (num8 - (num * num8)) + (num2 * z);
            result.M13 = (num7 - (num * num7)) - (num2 * y);
            result.M21 = (num8 - (num * num8)) - (num2 * z);
            result.M22 = num10 + (num * (Fixed64.One - num10));
            result.M23 = (num6 - (num * num6)) + (num2 * x);
            result.M31 = (num7 - (num * num7)) + (num2 * y);
            result.M32 = (num6 - (num * num6)) - (num2 * x);
            result.M33 = num9 + (num * (Fixed64.One - num9));
        }

        /// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>The resulting rotation matrix</returns>
        public static Fixed64Matrix AngleAxis(Fixed64 angle, Fixed64Vector3 axis)
        {
            CreateFromAxisAngle(ref axis, angle, out Fixed64Matrix result);
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", M11.RawValue, M12.RawValue, M13.RawValue, M21.RawValue, M22.RawValue, M23.RawValue, M31.RawValue, M32.RawValue, M33.RawValue);
        }

        /// <summary>
        /// Changes every sign of the matrix entry to '+'
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="result">The absolute matrix.</param>
        public static void Absolute(ref Fixed64Matrix matrix, out Fixed64Matrix result)
        {
            result.M11 = Fixed64.Abs(matrix.M11);
            result.M12 = Fixed64.Abs(matrix.M12);
            result.M13 = Fixed64.Abs(matrix.M13);
            result.M21 = Fixed64.Abs(matrix.M21);
            result.M22 = Fixed64.Abs(matrix.M22);
            result.M23 = Fixed64.Abs(matrix.M23);
            result.M31 = Fixed64.Abs(matrix.M31);
            result.M32 = Fixed64.Abs(matrix.M32);
            result.M33 = Fixed64.Abs(matrix.M33);
        }
    }
}

#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License

namespace Numerics.Fixed
{
    using System;

    /// <summary>
    /// Represents a vector with two Q31.32 fixed-point values.
    /// </summary>
    [Serializable]
    public struct Fixed64Vector2 : IEquatable<Fixed64Vector2>
    {
        private static Fixed64Vector2 zeroVector = new Fixed64Vector2(0, 0);
        public Fixed64 x;
        public Fixed64 y;

        public static Fixed64Vector2 Zero => zeroVector;

        public static Fixed64Vector2 One { get; } = new Fixed64Vector2(1, 1);

        public static Fixed64Vector2 Right { get; } = new Fixed64Vector2(1, 0);

        public static Fixed64Vector2 Left { get; } = new Fixed64Vector2(-1, 0);

        public static Fixed64Vector2 Up { get; } = new Fixed64Vector2(0, 1);

        public static Fixed64Vector2 Down { get; } = new Fixed64Vector2(0, -1);

        /// <summary>
        /// Constructor foe standard 2D vector.
        /// </summary>
        /// <param name="x">
        /// A <see cref="Single"/>
        /// </param>
        /// <param name="y">
        /// A <see cref="Single"/>
        /// </param>
        public Fixed64Vector2(Fixed64 x, Fixed64 y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructor for "square" vector.
        /// </summary>
        /// <param name="value">
        /// A <see cref="Single"/>
        /// </param>
        public Fixed64Vector2(Fixed64 value)
        {
            x = value;
            y = value;
        }

        public void Set(Fixed64 x, Fixed64 y)
        {
            this.x = x;
            this.y = y;
        }

        public static void Reflect(ref Fixed64Vector2 vector, ref Fixed64Vector2 normal, out Fixed64Vector2 result)
        {
            Fixed64 dot = Dot(vector, normal);
            result.x = vector.x - ((2f * dot) * normal.x);
            result.y = vector.y - ((2f * dot) * normal.y);
        }

        public static Fixed64Vector2 Reflect(Fixed64Vector2 vector, Fixed64Vector2 normal)
        {
            Reflect(ref vector, ref normal, out Fixed64Vector2 result);
            return result;
        }

        public static Fixed64Vector2 Add(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            return value1;
        }

        public static void Add(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = value1.x + value2.x;
            result.y = value1.y + value2.y;
        }

        public static Fixed64Vector2 Barycentric(Fixed64Vector2 value1, Fixed64Vector2 value2, Fixed64Vector2 value3, Fixed64 amount1, Fixed64 amount2)
        {
            return new Fixed64Vector2(
                FixedMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
                FixedMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
        }

        public static void Barycentric(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, ref Fixed64Vector2 value3, Fixed64 amount1,
                                       Fixed64 amount2, out Fixed64Vector2 result)
        {
            result = new Fixed64Vector2(
                FixedMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
                FixedMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
        }

        public static Fixed64Vector2 CatmullRom(Fixed64Vector2 value1, Fixed64Vector2 value2, Fixed64Vector2 value3, Fixed64Vector2 value4, Fixed64 amount)
        {
            return new Fixed64Vector2(
                FixedMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
                FixedMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
        }

        public static void CatmullRom(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, ref Fixed64Vector2 value3, ref Fixed64Vector2 value4,
                                      Fixed64 amount, out Fixed64Vector2 result)
        {
            result = new Fixed64Vector2(
                FixedMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
                FixedMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
        }

        public static Fixed64Vector2 Clamp(Fixed64Vector2 value1, Fixed64Vector2 min, Fixed64Vector2 max)
        {
            return new Fixed64Vector2(
                FixedMath.Clamp(value1.x, min.x, max.x),
                FixedMath.Clamp(value1.y, min.y, max.y));
        }

        public static void Clamp(ref Fixed64Vector2 value1, ref Fixed64Vector2 min, ref Fixed64Vector2 max, out Fixed64Vector2 result)
        {
            result = new Fixed64Vector2(
                FixedMath.Clamp(value1.x, min.x, max.x),
                FixedMath.Clamp(value1.y, min.y, max.y));
        }

        /// <summary>
        /// Returns FP precison distanve between two vectors
        /// </summary>
        /// <param name="value1">
        /// A <see cref="Fixed64Vector2"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="Fixed64Vector2"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Single"/>
        /// </returns>
        public static Fixed64 Distance(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            DistanceSquared(ref value1, ref value2, out Fixed64 result);
            return Fixed64.Sqrt(result);
        }


        public static void Distance(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64 result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = Fixed64.Sqrt(result);
        }

        public static Fixed64 DistanceSquared(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            DistanceSquared(ref value1, ref value2, out Fixed64 result);
            return result;
        }

        public static void DistanceSquared(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64 result)
        {
            result = (value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y);
        }

        /// <summary>
        /// Devide first vector with the secund vector
        /// </summary>
        /// <param name="value1">
        /// A <see cref="Fixed64Vector2"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="Fixed64Vector2"/>
        /// </param>
        /// <returns>
        /// A <see cref="Fixed64Vector2"/>
        /// </returns>
        public static Fixed64Vector2 Divide(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            return value1;
        }

        public static void Divide(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = value1.x / value2.x;
            result.y = value1.y / value2.y;
        }

        public static Fixed64Vector2 Divide(Fixed64Vector2 value1, Fixed64 divider)
        {
            Fixed64 factor = 1 / divider;
            value1.x *= factor;
            value1.y *= factor;
            return value1;
        }

        public static void Divide(ref Fixed64Vector2 value1, Fixed64 divider, out Fixed64Vector2 result)
        {
            Fixed64 factor = 1 / divider;
            result.x = value1.x * factor;
            result.y = value1.y * factor;
        }

        public static Fixed64 Dot(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return value1.x * value2.x + value1.y * value2.y;
        }

        public static void Dot(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64 result)
        {
            result = value1.x * value2.x + value1.y * value2.y;
        }

        public override bool Equals(object obj)
        {
            return (obj is Fixed64Vector2) ? this == ((Fixed64Vector2)obj) : false;
        }

        public bool Equals(Fixed64Vector2 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)(x + y);
        }

        public static Fixed64Vector2 Hermite(Fixed64Vector2 value1, Fixed64Vector2 tangent1, Fixed64Vector2 value2, Fixed64Vector2 tangent2, Fixed64 amount)
        {
            Fixed64Vector2 result = new Fixed64Vector2();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        public static void Hermite(ref Fixed64Vector2 value1, ref Fixed64Vector2 tangent1, ref Fixed64Vector2 value2, ref Fixed64Vector2 tangent2,
                                   Fixed64 amount, out Fixed64Vector2 result)
        {
            result.x = FixedMath.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
            result.y = FixedMath.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
        }

        public Fixed64 Magnitude
        {
            get
            {
                DistanceSquared(ref this, ref zeroVector, out Fixed64 result);
                return Fixed64.Sqrt(result);
            }
        }

        public static Fixed64Vector2 ClampMagnitude(Fixed64Vector2 vector, Fixed64 maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        public Fixed64 LengthSquared()
        {
            DistanceSquared(ref this, ref zeroVector, out Fixed64 result);
            return result;
        }

        public static Fixed64Vector2 Lerp(Fixed64Vector2 value1, Fixed64Vector2 value2, Fixed64 amount)
        {
            amount = FixedMath.Clamp(amount, 0, 1);

            return new Fixed64Vector2(
                FixedMath.Lerp(value1.x, value2.x, amount),
                FixedMath.Lerp(value1.y, value2.y, amount));
        }

        public static Fixed64Vector2 LerpUnclamped(Fixed64Vector2 value1, Fixed64Vector2 value2, Fixed64 amount)
        {
            return new Fixed64Vector2(
                FixedMath.Lerp(value1.x, value2.x, amount),
                FixedMath.Lerp(value1.y, value2.y, amount));
        }

        public static void LerpUnclamped(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, Fixed64 amount, out Fixed64Vector2 result)
        {
            result = new Fixed64Vector2(
                FixedMath.Lerp(value1.x, value2.x, amount),
                FixedMath.Lerp(value1.y, value2.y, amount));
        }

        public static Fixed64Vector2 Max(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return new Fixed64Vector2(
                FixedMath.Max(value1.x, value2.x),
                FixedMath.Max(value1.y, value2.y));
        }

        public static void Max(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = FixedMath.Max(value1.x, value2.x);
            result.y = FixedMath.Max(value1.y, value2.y);
        }

        public static Fixed64Vector2 Min(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return new Fixed64Vector2(
                FixedMath.Min(value1.x, value2.x),
                FixedMath.Min(value1.y, value2.y));
        }

        public static void Min(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = FixedMath.Min(value1.x, value2.x);
            result.y = FixedMath.Min(value1.y, value2.y);
        }

        public void Scale(Fixed64Vector2 other)
        {
            x = x * other.x;
            y = y * other.y;
        }

        public static Fixed64Vector2 Scale(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            Fixed64Vector2 result;
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;

            return result;
        }

        public static Fixed64Vector2 Multiply(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x *= value2.x;
            value1.y *= value2.y;
            return value1;
        }

        public static Fixed64Vector2 Multiply(Fixed64Vector2 value1, Fixed64 scaleFactor)
        {
            value1.x *= scaleFactor;
            value1.y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Fixed64Vector2 value1, Fixed64 scaleFactor, out Fixed64Vector2 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
        }

        public static void Multiply(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = value1.x * value2.x;
            result.y = value1.y * value2.y;
        }

        public static Fixed64Vector2 Negate(Fixed64Vector2 value)
        {
            value.x = -value.x;
            value.y = -value.y;
            return value;
        }

        public static void Negate(ref Fixed64Vector2 value, out Fixed64Vector2 result)
        {
            result.x = -value.x;
            result.y = -value.y;
        }

        public void Normalize()
        {
            Normalize(ref this, out this);
        }

        public static Fixed64Vector2 Normalize(Fixed64Vector2 value)
        {
            Normalize(ref value, out value);
            return value;
        }

        public Fixed64Vector2 Normalized
        {
            get
            {
                Normalize(ref this, out Fixed64Vector2 result);

                return result;
            }
        }

        public static void Normalize(ref Fixed64Vector2 value, out Fixed64Vector2 result)
        {
            DistanceSquared(ref value, ref zeroVector, out Fixed64 factor);
            factor = 1f / Fixed64.Sqrt(factor);
            result.x = value.x * factor;
            result.y = value.y * factor;
        }

        public static Fixed64Vector2 SmoothStep(Fixed64Vector2 value1, Fixed64Vector2 value2, Fixed64 amount)
        {
            return new Fixed64Vector2(
                FixedMath.SmoothStep(value1.x, value2.x, amount),
                FixedMath.SmoothStep(value1.y, value2.y, amount));
        }

        public static void SmoothStep(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, Fixed64 amount, out Fixed64Vector2 result)
        {
            result = new Fixed64Vector2(
                FixedMath.SmoothStep(value1.x, value2.x, amount),
                FixedMath.SmoothStep(value1.y, value2.y, amount));
        }

        public static Fixed64Vector2 Subtract(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            return value1;
        }

        public static void Subtract(ref Fixed64Vector2 value1, ref Fixed64Vector2 value2, out Fixed64Vector2 result)
        {
            result.x = value1.x - value2.x;
            result.y = value1.y - value2.y;
        }

        public static Fixed64 Angle(Fixed64Vector2 a, Fixed64Vector2 b)
        {
            return Fixed64.Acos(a.Normalized * b.Normalized) * Fixed64.Rad2Deg;
        }

        public static Fixed64 Angle360(Fixed64Vector2 a, Fixed64Vector2 b)
        {
            if (a == Fixed64Vector2.Zero && b == Fixed64Vector2.Zero) return 0;

            Fixed64Vector2 c = new Fixed64Vector2(b.x - a.x, b.y - a.y);
            Fixed64 xrd = FixedMath.Atan2(c.y, c.x);
            Fixed64 rot = xrd / FixedMath.Pi * 180;
            Fixed64 angle = 270 - rot;
            angle = angle < 0 ? 360 : angle;
            angle %= 360;

            return angle;
        }

        public Fixed64Vector3 ToTSVector()
        {
            return new Fixed64Vector3(x, y, 0);
        }

        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1})", x.AsFloat(), y.AsFloat());
        }

        public static Fixed64Vector2 operator -(Fixed64Vector2 value)
        {
            value.x = -value.x;
            value.y = -value.y;
            return value;
        }


        public static bool operator ==(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return value1.x == value2.x && value1.y == value2.y;
        }


        public static bool operator !=(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return value1.x != value2.x || value1.y != value2.y;
        }


        public static Fixed64Vector2 operator +(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x += value2.x;
            value1.y += value2.y;
            return value1;
        }


        public static Fixed64Vector2 operator -(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x -= value2.x;
            value1.y -= value2.y;
            return value1;
        }


        public static Fixed64 operator *(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            return Dot(value1, value2);
        }


        public static Fixed64Vector2 operator *(Fixed64Vector2 value, Fixed64 scaleFactor)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }


        public static Fixed64Vector2 operator *(Fixed64 scaleFactor, Fixed64Vector2 value)
        {
            value.x *= scaleFactor;
            value.y *= scaleFactor;
            return value;
        }


        public static Fixed64Vector2 operator /(Fixed64Vector2 value1, Fixed64Vector2 value2)
        {
            value1.x /= value2.x;
            value1.y /= value2.y;
            return value1;
        }


        public static Fixed64Vector2 operator /(Fixed64Vector2 value1, Fixed64 divider)
        {
            Fixed64 factor = 1 / divider;
            value1.x *= factor;
            value1.y *= factor;
            return value1;
        }
    }
}

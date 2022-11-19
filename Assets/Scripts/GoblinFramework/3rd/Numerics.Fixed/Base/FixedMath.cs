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
    /// Provides constants and static methods for trigonometric, logarithmic, and other common mathematical functions.
    /// </summary>
    public sealed class FixedMath
    {
        /// <summary>
        /// PI constant.
        /// </summary>
        public static Fixed64 Pi = Fixed64.Pi;

        /// <summary>
        /// PI over 2 constant.
        /// </summary>
        public static Fixed64 PiOver2 = Fixed64.PiOver2;

        /// <summary>
        /// A small value often used to decide if numeric results are zero.
        /// </summary>
        public static Fixed64 Epsilon = Fixed64.Epsilon;

        /// <summary>
        /// Degree to radians constant.
        /// </summary>
        public static Fixed64 Deg2Rad = Fixed64.Deg2Rad;

        /// <summary>
        /// Radians to degree constant.
        /// </summary>
        public static Fixed64 Rad2Deg = Fixed64.Rad2Deg;

        /// <summary>
        /// Gets the square root.
        /// </summary>
        /// <param name="number">The number to get the square root from.</param>
        /// <returns></returns>
        public static Fixed64 Sqrt(Fixed64 number)
        {
            return Fixed64.Sqrt(number);
        }

        /// <summary>
        /// Gets the maximum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the largest value.</returns>
        public static Fixed64 Max(Fixed64 val1, Fixed64 val2)
        {
            return (val1 > val2) ? val1 : val2;
        }

        /// <summary>
        /// Gets the minimum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the smallest value.</returns>
        public static Fixed64 Min(Fixed64 val1, Fixed64 val2)
        {
            return (val1 < val2) ? val1 : val2;
        }

        /// <summary>
        /// Gets the maximum number of three values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>Returns the largest value.</returns>
        public static Fixed64 Max(Fixed64 val1, Fixed64 val2, Fixed64 val3)
        {
            Fixed64 max12 = (val1 > val2) ? val1 : val2;
            return (max12 > val3) ? max12 : val3;
        }

        /// <summary>
        /// Returns a number which is within [min,max]
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static Fixed64 Clamp(Fixed64 value, Fixed64 min, Fixed64 max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Returns the sine of value.
        /// </summary>
        public static Fixed64 Sin(Fixed64 value)
        {
            return Fixed64.Sin(value);
        }

        /// <summary>
        /// Returns the cosine of value.
        /// </summary>
        public static Fixed64 Cos(Fixed64 value)
        {
            return Fixed64.Cos(value);
        }

        /// <summary>
        /// Returns the tan of value.
        /// </summary>
        public static Fixed64 Tan(Fixed64 value)
        {
            return Fixed64.Tan(value);
        }

        /// <summary>
        /// Returns the arc sine of value.
        /// </summary>
        public static Fixed64 Asin(Fixed64 value)
        {
            return Fixed64.Asin(value);
        }

        /// <summary>
        /// Returns the arc cosine of value.
        /// </summary>
        public static Fixed64 Acos(Fixed64 value)
        {
            return Fixed64.Acos(value);
        }

        /// <summary>
        /// Returns the arc tan of value.
        /// </summary>
        public static Fixed64 Atan(Fixed64 value)
        {
            return Fixed64.Atan(value);
        }

        /// <summary>
        /// Returns the arc tan of coordinates x-y.
        /// </summary>
        public static Fixed64 Atan2(Fixed64 y, Fixed64 x)
        {
            return Fixed64.Atan2(y, x);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fixed64 Floor(Fixed64 value)
        {
            return Fixed64.Floor(value);
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static Fixed64 Ceiling(Fixed64 value)
        {
            return value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static Fixed64 Round(Fixed64 value)
        {
            return Fixed64.Round(value);
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(Fixed64 value)
        {
            return Fixed64.Sign(value);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static Fixed64 Abs(Fixed64 value)
        {
            return Fixed64.Abs(value);
        }

        public static Fixed64 Barycentric(Fixed64 value1, Fixed64 value2, Fixed64 value3, Fixed64 amount1, Fixed64 amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static Fixed64 CatmullRom(Fixed64 value1, Fixed64 value2, Fixed64 value3, Fixed64 value4, Fixed64 amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using FPs not to lose precission
            Fixed64 amountSquared = amount * amount;
            Fixed64 amountCubed = amountSquared * amount;
            return (0.5 * (2.0 * value2 +
                                 (value3 - value1) * amount +
                                 (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                 (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        public static Fixed64 Distance(Fixed64 value1, Fixed64 value2)
        {
            return Fixed64.Abs(value1 - value2);
        }

        public static Fixed64 Hermite(Fixed64 value1, Fixed64 tangent1, Fixed64 value2, Fixed64 tangent2, Fixed64 amount)
        {
            // All transformed to FP not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            Fixed64 v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            Fixed64 sCubed = s * s * s;
            Fixed64 sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return result;
        }

        public static Fixed64 Lerp(Fixed64 value1, Fixed64 value2, Fixed64 amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static Fixed64 SmoothStep(Fixed64 value1, Fixed64 value2, Fixed64 amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            Fixed64 result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }
    }
}

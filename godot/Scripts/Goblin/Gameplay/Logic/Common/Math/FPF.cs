#if !FP
using System;
using System.IO;

namespace Kowtow.Math
{
    public partial struct FP : IEquatable<FP>, IComparable<FP>
    {
        private float rawvalue;
        public float RawValue => rawvalue;
        public const float MAX_VALUE = float.MaxValue;
        public const float MIN_VALUE = float.MinValue;
        public const float ONE = 1f;
        public const float TEN = 10f;
        public const float HALF = 0.5f;
        public const float PI_TIMES_2 = 6.28318530718f;
        public const float PI = 3.141592653589793f;
        public const float PI_OVER_2 = 1.5707963267948966f;

        public static readonly FP MaxValue = new FP(MAX_VALUE - 1);
        public static readonly FP MinValue = new FP(MIN_VALUE + 2);
        public static readonly FP One = new FP(ONE);
        public static readonly FP Ten = new FP(TEN);
        public static readonly FP Half = new FP(HALF);

        public static readonly FP Zero = new FP();
        public static readonly FP PositiveInfinity = new FP(MAX_VALUE);
        public static readonly FP NegativeInfinity = new FP(MIN_VALUE + 1);
        public static readonly FP NaN = new FP(MIN_VALUE);

        public static readonly FP EN1 = One / 10;
        public static readonly FP EN2 = One / 100;
        public static readonly FP EN3 = One / 1000;
        public static readonly FP EN4 = One / 10000;
        public static readonly FP EN5 = One / 100000;
        public static readonly FP EN6 = One / 1000000;
        public static readonly FP EN7 = One / 10000000;
        public static readonly FP EN8 = One / 100000000;
        public static readonly FP Epsilon = EN3;

        /// <summary>
        /// The value of Pi
        /// </summary>
        public static readonly FP Pi = new FP(PI);
        public static readonly FP PiOver2 = new FP(PI_OVER_2);
        public static readonly FP PiTimes2 = new FP(PI_TIMES_2);
        public static readonly FP PiInv = (FP)0.3183098861837906715377675267M;
        public static readonly FP PiOver2Inv = (FP)0.6366197723675813430755350535M;

        public static readonly FP Deg2Rad = Pi / new FP(180);

        public static readonly FP Rad2Deg = new FP(180) / Pi;

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// </summary>
        internal static FP Pow2(FP x)
        {
            return new FP((float)System.Math.Pow(2, x.rawvalue));
        }

        public static FP Log(FP x)
        {
            return new FP(System.Math.Log(x.AsFloat()));
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static FP Log2(FP x)
        {
            return Log(x) / Log(2);
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(FP value)
        {
            return value.rawvalue < 0 ? -1 : value.rawvalue > 0 ? 1 : 0;
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static FP Abs(FP value)
        {
            return new FP(System.Math.Abs(value.rawvalue));
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// </summary>
        public static FP FastAbs(FP value)
        {
            return Abs(value);
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static FP Floor(FP value)
        {
            return new FP(System.Math.Floor(value.rawvalue));
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static FP Ceiling(FP value)
        {
            return new FP(System.Math.Ceiling(value.rawvalue));
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static FP Round(FP value)
        {
            return new FP(System.Math.Round(value.rawvalue));
        }

        public static FP Max(FP left, FP right)
        {
            return new FP(System.Math.Max(left.rawvalue, right.rawvalue));
        }

        public static FP Min(FP left, FP right)
        {
            return new FP(System.Math.Min(left.rawvalue, right.rawvalue));
        }

        public static FP Clamp(FP origin, FP min, FP max)
        {
            return Min(Max(origin, min), max);
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static FP operator +(FP x, FP y)
        {
            return new FP(x.rawvalue + y.rawvalue);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static FP operator -(FP x, FP y)
        {
            return new FP(x.rawvalue - y.rawvalue);
        }

        /// <summary>
        /// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static FP FastSub(FP x, FP y)
        {
            return x - y;
        }

        public static FP operator *(FP x, FP y)
        {
            return new FP(x.rawvalue * y.rawvalue);
        }
        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static FP FastMul(FP x, FP y)
        {
            return x * y;
        }

        //[MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
        public static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0)
            {
                result += 4;
                x <<= 4;
            }
            while ((x & 0x8000000000000000) == 0)
            {
                result += 1;
                x <<= 1;
            }
            return result;
        }

        public static FP operator /(FP x, FP y)
        {
            return new FP(x.rawvalue / y.rawvalue);
        }

        public static FP operator %(FP x, FP y)
        {
            return new FP(x.rawvalue%y.rawvalue);
        }

        public static FP operator -(FP x)
        {
            return new FP(-x.rawvalue);
        }

        public static bool operator ==(FP x, FP y)
        {
            return x.rawvalue.Equals(y.rawvalue);
        }

        public static bool operator !=(FP x, FP y)
        {
            return false == x.rawvalue.Equals(y.rawvalue);
        }

        public static bool operator >(FP x, FP y)
        {
            return x.rawvalue > y.rawvalue;
        }

        public static bool operator <(FP x, FP y)
        {
            return x.rawvalue < y.rawvalue;
        }

        public static bool operator >=(FP x, FP y)
        {
            return x.rawvalue >= y.rawvalue;
        }

        public static bool operator <=(FP x, FP y)
        {
            return x.rawvalue <= y.rawvalue;
        }


        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static FP Sqrt(FP x)
        {
            return new FP(System.Math.Sqrt(x.rawvalue));
        }

        /// <summary>
        /// Returns the Sine of x.
        /// This function has about 9 decimals of accuracy for small values of x.
        /// It may lose accuracy as the value of x grows.
        /// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
        /// </summary>
        public static FP Sin(FP x)
        {
            return new FP(System.Math.Sin(x.rawvalue));
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static FP FastSin(FP x)
        {
            return Sin(x);
        }

        /// <summary>
        /// Returns the cosine of x.
        /// See Sin() for more details.
        /// </summary>
        public static FP Cos(FP x)
        {
            return new FP(System.Math.Cos(x.rawvalue));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static FP FastCos(FP x)
        {
            return Cos(x);
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static FP Tan(FP x)
        {
            return new FP(System.Math.Tan(x.rawvalue));
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static FP Atan(FP z)
        {
            return new FP(System.Math.Atan(z.rawvalue));
        }

        public static FP Atan2(FP y, FP x)
        {
            return new FP(System.Math.Atan2(y.rawvalue, x.rawvalue));
        }

        public static FP Asin(FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static FP Acos(FP x)
        {
            return new FP(System.Math.Acos(x.rawvalue));
        }

        public static implicit operator FP(long value)
        {
            return new FP(value);
        }

        public static explicit operator long(FP value)
        {
            return (long)value.rawvalue;
        }

        public static explicit operator float(FP value)
        {
            return value.rawvalue;
        }

        public static explicit operator FP(decimal value)
        {
            return new FP((float)value);
        }

        public static implicit operator FP(int value)
        {
            return new FP(value);
        }

        public static explicit operator decimal(FP value)
        {
            return (decimal)value.rawvalue;
        }

        public float AsFloat()
        {
            return (float)this;
        }

        public int AsInt()
        {
            return (int)this;
        }

        public uint AsUInt()
        {
            return (uint)this;
        }

        public long AsLong()
        {
            return (long)this;
        }

        public double AsDouble()
        {
            return (double)this;
        }

        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        public static float ToFloat(FP value)
        {
            return (float)value;
        }

        public static int ToInt(FP value)
        {
            return (int)value;
        }

        public static uint ToUInt(FP value)
        {
            return (uint)value;
        }

        public static bool IsInfinity(FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        public static bool IsNaN(FP value)
        {
            return value == NaN;
        }

        public override bool Equals(object obj)
        {
            return obj is FP _obj && _obj.rawvalue.Equals(rawvalue);
        }

        public override int GetHashCode()
        {
            return rawvalue.GetHashCode();
        }

        public bool Equals(FP other)
        {
            return rawvalue.Equals(other.rawvalue);
        }

        public int CompareTo(FP other)
        {
            return rawvalue.CompareTo(other.rawvalue);
        }

        public override string ToString()
        {
            return rawvalue.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }
        
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        public static FP FromRaw(long rawValue)
        {
            return new FP(rawValue);
        }

        FP(float val)
        {
            this.rawvalue = val;
        }

        FP(double val)
        {
            this.rawvalue = (float)val;
        }

        FP(long val)
        {
            this.rawvalue = val;
        }

        public FP(int rawvalue)
        {
            this.rawvalue = rawvalue;
        }
    }
}
#endif
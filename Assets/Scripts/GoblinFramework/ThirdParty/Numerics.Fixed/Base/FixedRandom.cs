namespace Numerics.Fixed
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a random number generator based on a deterministic approach.
    /// </summary>
    public class FixedRandom
    {
        // From http://www.codeproject.com/Articles/164087/Random-Number-Generation
        // Class TSRandom generates random numbers
        // from a uniform distribution using the Mersenne
        // Twister algorithm.
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0dfU;
        private const uint UPPER_MASK = 0x80000000U;
        private const uint LOWER_MASK = 0x7fffffffU;
        private const int MAX_RAND_INT = 0x7fffffff;
        private readonly uint[] mag01 = { 0x0U, MATRIX_A };
        private readonly uint[] mt = new uint[N];
        private int mti = N + 1;

        public static readonly IReadOnlyCollection<string> trackingPaths = new List<string> { nameof(mt), nameof(mti) }.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRandom"/> class, using a time-dependent default seed value.
        /// </summary>
        public FixedRandom()
        {
            Init_genrand((uint)DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRandom"/> class, using the specified seed value.
        /// </summary>
        public FixedRandom(int seed)
        {
            Init_genrand((uint)seed);
        }

        public FixedRandom(int[] init)
        {
            uint[] initArray = new uint[init.Length];
            for (int i = 0; i < init.Length; ++i)
                initArray[i] = (uint)init[i];
            Init_by_array(initArray, (uint)initArray.Length);
        }

        public static int MaxRandomInt => 0x7fffffff;

        /// <summary>
        /// Returns a random integer.
        /// </summary>
        public int Next()
        {
            return Genrand_int31();
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                int tmp = maxValue;
                maxValue = minValue;
                minValue = tmp;
            }

            int range = maxValue - minValue;

            return minValue + Next() % range;
        }

        /// <summary>
        /// Returns a random <see cref="Fixed64"/> that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public Fixed64 Next(float minValue, float maxValue)
        {
            int minValueInt = (int)(minValue * 1000), maxValueInt = (int)(maxValue * 1000);

            if (minValueInt > maxValueInt)
            {
                int tmp = maxValueInt;
                maxValueInt = minValueInt;
                minValueInt = tmp;
            }

            return (Fixed64.Floor((maxValueInt - minValueInt + 1) * NextFP() +
                minValueInt)) / 1000;
        }

        /// <summary>
        /// Returns a random <see cref="Fixed64"/> number between 0.0 and 1.0.
        /// </summary>
        public Fixed64 NextFP() => ((Fixed64)Next()) / (MaxRandomInt);

        private float NextFloat()
        {
            return (float)Genrand_real2();
        }

        private float NextFloat(bool includeOne)
        {
            if (includeOne)
            {
                return (float)Genrand_real1();
            }
            return (float)Genrand_real2();
        }

        private float NextFloatPositive()
        {
            return (float)Genrand_real3();
        }

        private double NextDouble()
        {
            return Genrand_real2();
        }

        private double NextDouble(bool includeOne)
        {
            if (includeOne)
            {
                return Genrand_real1();
            }
            return Genrand_real2();
        }

        private double NextDoublePositive()
        {
            return Genrand_real3();
        }

        private double Next53BitRes()
        {
            return Genrand_res53();
        }

        public void Initialize()
        {
            Init_genrand((uint)DateTime.Now.Millisecond);
        }

        public void Initialize(int seed)
        {
            Init_genrand((uint)seed);
        }

        public void Initialize(int[] init)
        {
            uint[] initArray = new uint[init.Length];
            for (int i = 0; i < init.Length; ++i)
                initArray[i] = (uint)init[i];
            Init_by_array(initArray, (uint)initArray.Length);
        }

        private void Init_genrand(uint s)
        {
            mt[0] = s & 0xffffffffU;
            for (mti = 1; mti < N; mti++)
            {
                mt[mti] = (uint)(1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
                mt[mti] &= 0xffffffffU;
            }
        }

        private void Init_by_array(uint[] init_key, uint key_length)
        {
            int i, j, k;
            Init_genrand(19650218U);
            i = 1;
            j = 0;
            k = (int)(N > key_length ? N : key_length);
            for (; k > 0; k--)
            {
                mt[i] = (uint)((mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525U)) + init_key[j] + j);
                mt[i] &= 0xffffffffU;
                i++;
                j++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
                if (j >= key_length)
                    j = 0;
            }
            for (k = N - 1; k > 0; k--)
            {
                mt[i] = (uint)((mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) *
                    1566083941U)) - i);
                mt[i] &= 0xffffffffU;
                i++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
            }
            mt[0] = 0x80000000U;
        }

        uint Genrand_int32()
        {
            uint y;
            if (mti >= N)
            {
                int kk;
                if (mti == N + 1)
                    Init_genrand(5489U);
                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
                }
                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];
                mti = 0;
            }
            y = mt[mti++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);
            return y;
        }

        private int Genrand_int31()
        {
            return (int)(Genrand_int32() >> 1);
        }

        Fixed64 Genrand_FP()
        {
            return Genrand_int32() * (Fixed64.One / 4294967295);
        }

        double Genrand_real1()
        {
            return Genrand_int32() * (1.0 / 4294967295.0);
        }
        double Genrand_real2()
        {
            return Genrand_int32() * (1.0 / 4294967296.0);
        }

        double Genrand_real3()
        {
            return (Genrand_int32() + 0.5) * (1.0 / 4294967296.0);
        }

        double Genrand_res53()
        {
            uint a = Genrand_int32() >> 5, b = Genrand_int32() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }
    }
}

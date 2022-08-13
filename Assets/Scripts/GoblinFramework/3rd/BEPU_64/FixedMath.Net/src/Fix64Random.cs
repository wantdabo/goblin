using System;

namespace FixMath.NET
{
	public class Fix64Random
    {
        private Random random;

        public Fix64Random(int seed)
        {
            random = new Random(seed);
        }

        public Fix64 Next()
        {
            Fix64 result = new Fix64();
            result.RawValue = (uint)random.Next(int.MinValue, int.MaxValue);
            return result;
        }

        public Fix64 NextInt(int maxValue)
        {
            return random.Next(maxValue);
        }
    }
}

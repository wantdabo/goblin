using FixMath.NET;

namespace BEPUutilities
{
	public static class F64
	{
		public static readonly Fix64 C0 = (Fix64)0;
		public static readonly Fix64 C1 = (Fix64)1;
		public static readonly Fix64 C180 = (Fix64)180;
		public static readonly Fix64 C2 = (Fix64)2;
		public static readonly Fix64 C3 = (Fix64)3;
		public static readonly Fix64 C5 = (Fix64)5;
		public static readonly Fix64 C6 = (Fix64)6;
		public static readonly Fix64 C16 = (Fix64)16;
		public static readonly Fix64 C24 = (Fix64)24;
		public static readonly Fix64 C50 = (Fix64)50;
		public static readonly Fix64 C60 = (Fix64)60;
		public static readonly Fix64 C120 = (Fix64)120;
		public static readonly Fix64 C0p001 = (Fix64)0.001m;
		public static readonly Fix64 C0p5 = (Fix64)0.5m;
		public static readonly Fix64 C0p25 = (Fix64)0.25m;
		public static readonly Fix64 C1em09 = (Fix64)1e-9m;
		public static readonly Fix64 C1em9 = (Fix64)1e-9m;
		public static readonly Fix64 Cm1em9 = (Fix64)(-1e-9m);
		public static readonly Fix64 C1em14 = (Fix64)(1e-14m);		
		public static readonly Fix64 C0p1 = (Fix64)0.1m;
		public static readonly Fix64 OneThird = (Fix64)1/(Fix64)3;
		public static readonly Fix64 C0p75 = (Fix64)0.75m;
		public static readonly Fix64 C0p15 = (Fix64)0.15m;
		public static readonly Fix64 C0p3 = (Fix64)0.3m;
		public static readonly Fix64 C0p0625 = (Fix64)0.0625m;
		public static readonly Fix64 C0p99 = (Fix64).99m;
		public static readonly Fix64 C0p9 = (Fix64).9m;
		public static readonly Fix64 C1p5 = (Fix64)1.5m;
		public static readonly Fix64 C1p1 = (Fix64)1.1m;
        public static readonly Fix64 Half = Fix64.One / 2;
        public static readonly Fix64 OneEighth = Fix64.One / 8;
		public static readonly Fix64 FourThirds = new Fix64(4) / 3;
		public static readonly Fix64 TwoFifths = new Fix64(2) / 5;
		public static readonly Fix64 C0p2 = (Fix64)0.2m;
		public static readonly Fix64 C0p8 = (Fix64)0.8m;
		public static readonly Fix64 C0p01 = (Fix64)0.01m;
		public static readonly Fix64 C1em7 = (Fix64)1e-7m;
		public static readonly Fix64 C1em5 = (Fix64)1e-5m;
		public static readonly Fix64 C1em4 = (Fix64)1e-4m;
		public static readonly Fix64 C1em10 = (Fix64)1e-10m;
		public static readonly Fix64 Cm0p25 = (Fix64)(-0.25m);
		public static readonly Fix64 Cm0p9999 = (Fix64)(-0.9999m);
		public static readonly Fix64 C1m1em12 = Fix64.One - (Fix64)1e-12m;
		public static readonly Fix64 GoldenRatio = Fix64.One + Fix64.Sqrt((Fix64)5) / (Fix64)2;
		public static readonly Fix64 OneTwelfth = Fix64.One / (Fix64)12;
		public static readonly Fix64 C0p0833333333 = (Fix64).0833333333m;
		public static readonly Fix64 C90000 = (Fix64)90000;
		public static readonly Fix64 C600000 = (Fix64)600000;
	}
}

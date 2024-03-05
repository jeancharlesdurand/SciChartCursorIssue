using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciChart_FIFOScrollingCharts
{
    internal static class Helpers
    {
        public static double DateTimeToDouble(DateTime value)
        {
            long ticks = value.Ticks;
            if (ticks == 0L)
            {
                return 0.0;
            }
            if (ticks < 0xc92a69c000L)
            {
                ticks += 0x85103c0cb83c000L;
            }
            if (ticks < 0x6efdddaec64000L)
            {
                throw new OverflowException("Invalid OA Date");
            }
            long num2 = (ticks - 0x85103c0cb83c000L) / 0x2710L;
            if (num2 < 0L)
            {
                long num3 = num2 % 0x5265c00L;
                if (num3 != 0L)
                {
                    num2 -= (0x5265c00L + num3) * 2L;
                }
            }
            return (num2 / 86400000.0);
        }

        public static DateTime DoubleToDateTime(double value)
        {
            if ((value >= 2958466.0) || (value <= -657435.0))
            {
                throw new ArgumentException("Invalid OA Date");
            }
            var num = (long)((value * 86400000.0) + ((value >= 0.0) ? 0.5 : -0.5));
            if (num < 0L)
            {
                num -= (num % 0x5265c00L) * 2L;
            }
            num += 0x3680b5e1fc00L;
            if ((num < 0L) || (num >= 0x11efae44cb400L))
            {
                throw new ArgumentException("Invalid OA Date");
            }
            return new DateTime(num * 0x2710L);
        }
    }
}

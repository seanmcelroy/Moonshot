using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonshot.Common
{
    public static class RandomUtility
    {
        //returns a uniformly random ulong between ulong.Min inclusive and ulong.Max inclusive
        public static ulong NextULong(this Random Rng)
        {
            byte[] buf = new byte[8];
            Rng.NextBytes(buf);
            return BitConverter.ToUInt64(buf, 0);
        }

        //returns a uniformly random ulong between ulong.Min and Max without modulo bias
        public static ulong NextULong(this Random Rng, ulong Max, bool inclusiveUpperBound = false)
        {
            return Rng.NextULong(ulong.MinValue, Max, inclusiveUpperBound);
        }

        //returns a uniformly random ulong between Min and Max without modulo bias
        public static ulong NextULong(this Random Rng, ulong Min, ulong Max, bool inclusiveUpperBound = false)
        {
            ulong range = Max - Min;

            if (inclusiveUpperBound)
            {
                if (range == ulong.MaxValue)
                {
                    return Rng.NextULong();
                }

                range++;
            }

            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException("Max must be greater than Min when inclusiveUpperBound is false, and greater than or equal to when true", "Max");
            }

            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong r;
            do
            {
                r = Rng.NextULong();
            } while (r > limit);

            return r % range + Min;
        }

        //returns a uniformly random long between long.Min inclusive and long.Max inclusive
        public static long NextLong(this Random Rng)
        {
            byte[] buf = new byte[8];
            Rng.NextBytes(buf);
            return BitConverter.ToInt64(buf, 0);
        }

        //returns a uniformly random long between long.Min and Max without modulo bias
        public static long NextLong(this Random Rng, long Max, bool inclusiveUpperBound = false)
        {
            return Rng.NextLong(long.MinValue, Max, inclusiveUpperBound);
        }

        //returns a uniformly random long between Min and Max without modulo bias
        public static long NextLong(this Random Rng, long Min, long Max, bool inclusiveUpperBound = false)
        {
            ulong range = (ulong)(Max - Min);

            if (inclusiveUpperBound)
            {
                if (range == ulong.MaxValue)
                {
                    return Rng.NextLong();
                }

                range++;
            }

            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException("Max must be greater than Min when inclusiveUpperBound is false, and greater than or equal to when true", "Max");
            }

            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong r;
            do
            {
                r = Rng.NextULong();
            } while (r > limit);
            return (long)(r % range + (ulong)Min);
        }
    }
}

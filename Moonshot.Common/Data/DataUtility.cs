using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Moonshot.Common.Data
{
    internal static class DataUtility
    {
        private static ObjectCache _Cache = MemoryCache.Default;

        internal static Thing Load(string dbref)
        {
            var thing = _Cache[dbref] as Thing;
            if (thing != null)
                return thing;

            return LoadFromServer(dbref);
        }

        private static Thing LoadFromServer(string dbref)
        {
            return null;
        }
    }
}

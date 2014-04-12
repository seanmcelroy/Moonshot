using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonshot.Common
{
    using System.Runtime.CompilerServices;

    public class Program : Thing
    {
        public override string Id
        {
            get
            {
                return base.Id + "P";
            }
        }
        public string Name
        {
            get
            {
                return ((dynamic)this).name;
            }
        }

        public Func<IList<string>, object> Implementation { get; set; }
    }
}
namespace Moonshot.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public static class ProgramUtility
    {
        public static Thing CreateInternalProgram(
            Func<IList<string>, object> definition)
        {
            dynamic thisProg = new Thing();
            thisProg.name = "@serializejson";
            thisProg.verb = true;

            thisProg.impl = new ExpandoObject();
            thisProg.impl.Main = new Func<IList<string>, object>(definition);

            return thisProg;
        }
    }
}

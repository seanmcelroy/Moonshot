namespace Moonshot.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    public static class ProgramUtility
    {
        public static Program CreateInternalProgram(
            Func<IList<string>, object> definition)
        {
            var thisProg = new Program();
            thisProg.Implementation = definition;
            return thisProg;
        }
    }
}

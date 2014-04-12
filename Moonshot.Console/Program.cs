
namespace Moonshot.Console
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using csscript;
    using CSScriptLibrary;
    using Moonshot.Common;
    using Newtonsoft.Json;

    public class Program
    {
        private static readonly Dictionary<string, Thing> Things = new Dictionary<string, Thing>();

        public static void Main()
        {
            SetupVerbs();

            while (true)
            {
                Console.Write("\r\n> ");
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("Huh?");
                    continue;
                }

                var inputParts = input.Split(' ', '\t');

                if (Things.ContainsKey(inputParts[0].ToLowerInvariant()))
                {
                    var verb = Things[inputParts[0].ToLowerInvariant()] as Moonshot.Common.Program;
                    if (verb != null)
                    {
                        var result = verb.Implementation.Invoke(inputParts.Skip(1).ToList());
                        if (result is bool && (bool)result) return;
                    }
                    else
                        Console.WriteLine("Huh?");
                }
                else
                    Console.WriteLine("Huh?");
            }
        }

        private static void SetupVerbs()
        {
            Things.Add(
                "@prog",
                /* name body */
                ProgramUtility.CreateInternalProgram(
                    s =>
                        {
                            var body = s.Skip(1).Aggregate((c, n) => c + ' ' + n);

                            var program = new Moonshot.Common.Program();
                            ((dynamic)program).name = s[0];
                            ((dynamic)program).body = body;

                            Assembly assembly;
                            try
                            {
                                assembly = CSScript.LoadCode(@"using System;
                                    public class Script
                                    {
                                        public static object Main(object context)
                                        {
                                           " + body + @";return 0;
                                        }
                                    }");
                            }
                            catch (CompilerException ce)
                            {
                                Console.WriteLine(
                                    ce.Message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                                        .Last()
                                        .Split(':')
                                        .Skip(2)
                                        .Aggregate((c, n) => c + ':' + n));
                                return null;
                            }

                            var fid = new AsmHelper(assembly).GetMethodInvoker("Script.Main", new object());
                            program.Implementation = (IList<string> sx) => { return fid.Invoke(null, sx); }; //new Func<IList<string>,object>((IList<string> sx) => fid.Invoke(null, sx));

                            Console.WriteLine("Program {0}({1}) created.", s[0], program.Id);

                            Things.Add(s[0].ToLowerInvariant(), program);
                            return program;
                        }));

            Things.Add(
                "@serializejson",
                /* name */
                ProgramUtility.CreateInternalProgram(
                    s =>
                        {
                            var thing = Things[s[0].ToLowerInvariant()];
                            var serial = JsonConvert.SerializeObject(thing);
                            Console.WriteLine(serial);
                            return thing;
                        }));

            Things.Add(
                "@serializejsontest",
                /* name */
                ProgramUtility.CreateInternalProgram(
                    s =>
                        {
                            var thing = Things[s[0].ToLowerInvariant()];
                            var serial = JsonConvert.SerializeObject(thing);
                            Console.WriteLine(serial);
                            var thing2 = JsonConvert.DeserializeObject<Thing>(serial);
                            return thing2;
                        }));

            Things.Add(
                "register",
                /* username pass email */
                ProgramUtility.CreateInternalProgram(
                    s =>
                        {
                            dynamic player = new Thing();
                            player.name = s[0];
                            player.pass = s[1];
                            player.email = s[2];
                            Things.Add(player.name, player);
                            return (Thing)player;
                        }));
        }
    }
}

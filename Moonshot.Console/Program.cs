
namespace Moonshot.Console
{
    using System;
    using System.Collections.Generic;
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
                    var verb = (dynamic)Things[inputParts[0].ToLowerInvariant()];
                    if (verb != null && verb.verb)
                    {
                        var result = verb.impl.Main(inputParts.Skip(1).ToList());
                        if (result is bool && result == true) return;
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

                            dynamic program = new Thing();
                            program.name = s[0];
                            program.body = body;
                            program.verb = true;

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

                            program.impl = new AsmHelper(assembly).GetMethodInvoker("Script.Main", new object());

                            Console.WriteLine("Program {0}(#{1}) created.");

                            Things.Add(program.name.ToLowerInvariant(), program);
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

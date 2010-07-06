using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Runner {
    class Program {
        class Sample {
            public string Name { get; set; }
            public Action Action { get; set; }
        }
        [Sample("Main", ClassSample = false)]
        static void Main(string[] args) {
            string location = Assembly.GetExecutingAssembly().Location;

            var sample = typeof(SampleAttribute);
            var typeGroups = from f in Directory.GetParent(location).EnumerateFiles("*.dll")
                             let a = Assembly.LoadFrom(f.FullName)
                             where a.GetCustomAttributes(sample, false).Length != 0
                             from type in a.GetTypes()
                             let attr = type.GetCustomAttributes(sample, false).FirstOrDefault()
                             where attr != null
                             group type by ((SampleAttribute)attr).ClassSample;

            var classSamples = from grp in typeGroups
                               where grp.Key
                               from type in grp
                               select new Sample
                               {
                                   Name = type.GetCustomAttributes(sample, false).Cast<SampleAttribute>().First().SampleName,
                                   Action = () =>
                                   {
                                       dynamic instance = type.GetConstructor(new Type[0] { }).Invoke(new object[] { });
                                       instance.Execute();
                                   }
                               };

            var methodSamples = from grp in typeGroups
                                where !grp.Key
                                from type in grp
                                from method in type.GetMethods()
                                let attr = method.GetCustomAttributes(sample, false).FirstOrDefault()
                                where attr != null
                                select new Sample
                                {
                                    Name = ((SampleAttribute)attr).SampleName,
                                    Action = () =>
                                        {
                                            method.Invoke(null, null);
                                        }
                                };
            var quit = Enumerable.Repeat(new Sample { Name = "Quit", Action = () => { Environment.Exit(0); } }, 1);
            var samples = classSamples.Union(methodSamples).Concat(quit);
            var get = new Func<string, Func<int, bool>, int>((prompt, check) =>
            {
                while (true) {
                    Console.Write(prompt);
                    var res = default(int);
                    if (int.TryParse(Console.ReadLine(), out res) && check(res))
                        return res;
                }
            });

            while (true) {
                Console.WindowHeight = 6 + samples.Count();

                Console.Clear();

                Console.WriteLine("Rx samples :: bartde - April 2010");
                Console.WriteLine("---------------------------------");
                Console.WriteLine();

                Console.WriteLine("Pick your Rx poison:");

                var i = 1;
                foreach (var opt in samples)
                    Console.WriteLine("{0}. {1}", i++, opt.Name);
                Console.WriteLine();

                var c = get("Your choice: ", x => x >= 0 && x <= samples.Count());
                if (c == 0)
                    break;

                var s = samples.ElementAt(c - 1);

                Console.Clear();
                Console.WriteLine(s.Name);
                Console.WriteLine();
                s.Action();
                Console.WriteLine();
                Console.WriteLine("Sample finished. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}

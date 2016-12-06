namespace Roslynguist.Console
{
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Analyzers.MethodAnalyzers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var source = new AnalysisAggregateSource();
            var loaded = source.LoadSolution(@"C:\Repositories\SimpleFeed\ConsoleApplication1\ConsoleApplication1.sln").Result;
            var callMap = new MembersCallMap(source);
            callMap.BuildDependencyMap();

            foreach (var member in callMap._members
                .Where(m => m.Value.First().Callers.Count > 0 || m.Value.First().Callees.Count > 0))
            {
                Console.WriteLine("============================");
                Console.WriteLine(member.Key.ToDisplayString());
                Console.WriteLine(member.Value.First().MemberSyntax.ToString());

                Console.WriteLine("CALLERS");
                foreach (var caller in member.Value.First().Callers)
                {
                    Console.WriteLine(caller.ToDisplayString());
                }

                Console.WriteLine("CALLEES");
                foreach (var callee in member.Value.First().Callees)
                {
                    Console.WriteLine(callee.ToDisplayString());
                }

                Console.WriteLine("============================");
            }

            Console.ReadKey();
        }
    }
}

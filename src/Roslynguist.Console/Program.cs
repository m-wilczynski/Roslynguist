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
            //var result = analyst.Load().Result;
            //var projects =
            //    result[@"C:\Repositories\Localwire.Graphinder\Localwire.Graphinder.sln"].Projects.Select(
            //        p => p.GetCompilationAsync().Result).ToList();
            //var roots = projects.Select(p => p.SyntaxTrees.Select(st => st.GetRoot()).ToList()).ToList();
            //var semanticModel =
            //    projects.Select(p => p.SyntaxTrees.Select(st => p.GetSemanticModel(st)).ToList()).SelectMany(t => t).ToList();
            //var descendats2 =
            //    semanticModel.Select(sm => sm.SyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
            //        .Select(n => new { Method = n, Symbol = sm.GetDeclaredSymbol(n) })).SelectMany(t => t).ToList();


            var source = new AnalysisAggregateSource();
            var loaded = source.LoadSolution(@"C:\Repositories\Localwire.Graphinder\Localwire.Graphinder.sln").Result;
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

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var source = new AnalysisAggregateSource();
            //var result = source.LoadSolution(@"C:\Repositories\MAS.Photographers\MAS.Photographers.sln").Result;
            //result = source.LoadSolution(@"C:\Repositories\Localwire.Graphinder\Localwire.Graphinder.sln").Result;
            //stopwatch.Stop();
            //Console.WriteLine("DONE: "+stopwatch.Elapsed.TotalSeconds);
            //stopwatch.Reset();
            //stopwatch.Start();
            //var cmp = source.GetCompilationsPerSolution();
            //stopwatch.Stop();
            //Console.WriteLine("DONE: " + stopwatch.Elapsed.TotalSeconds);

            //Console.ReadKey();
        }
    }
}

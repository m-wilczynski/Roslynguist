namespace Roslynguist.Console
{
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var analyst = new SolutionLoader();
            analyst.AddSolutionToLoad(@"C:\Repositories\Localwire.Graphinder\Localwire.Graphinder.sln");
            var result = analyst.Load().Result;
            var projects =
                result[@"C:\Repositories\Localwire.Graphinder\Localwire.Graphinder.sln"].Projects.Select(
                    p => p.GetCompilationAsync().Result).ToList();
            var roots = projects.Select(p => p.SyntaxTrees.Select(st => st.GetRoot()).ToList()).ToList();
            var semanticModel =
                projects.Select(p => p.SyntaxTrees.Select(st => p.GetSemanticModel(st)).ToList()).SelectMany(t => t).ToList();
            var descendats2 =
                semanticModel.Select(sm => sm.SyntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(n => new { Method = n, Symbol = sm.GetDeclaredSymbol(n) })).SelectMany(t => t).ToList();

            Console.WriteLine(descendats2[0].Symbol);
            Console.WriteLine(descendats2[0].Method);

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

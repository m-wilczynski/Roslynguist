namespace Roslynguist.Analyzers.MethodAnalyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class MethodCallAnalyzer : SyntaxAnalyzer
    {

        public MethodCallAnalyzer(AnalysisAggregateSource source) : base(source)
        {

        }

        public bool WasMapBuilded { get; private set; }

        private void BuildDependencyMap()
        {
            if (WasMapBuilded) return;



            WasMapBuilded = true;
        }
    }
}

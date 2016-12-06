namespace Roslynguist.Analyzers.MethodAnalyzers
{
    using System;

    public class MethodCallAnalyzer : SyntaxAnalyzer
    {
        public readonly AnalysisAggregateSource AnalysisSource;

        public MethodCallAnalyzer(AnalysisAggregateSource source) : base(source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            AnalysisSource = source;
        }

        
    }
}

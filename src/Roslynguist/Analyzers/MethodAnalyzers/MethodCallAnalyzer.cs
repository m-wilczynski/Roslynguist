namespace Roslynguist.Analyzers.MethodAnalyzers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
    using Models;

    public class MethodCallAnalyzer : SyntaxAnalyzer
    {
        private readonly Dictionary<IMethodSymbol, MethodModel> _methods = new Dictionary<IMethodSymbol, MethodModel>();
        private readonly Dictionary<IFieldSymbol, FieldInitializerSyntax> _fields = new Dictionary<IFieldSymbol, FieldInitializerSyntax>();
        private readonly Dictionary<IPropertySymbol, PropertyBlockSyntax> _properties = new Dictionary<IPropertySymbol, PropertyBlockSyntax>();

        public readonly AnalysisAggregateSource AnalysisSource;

        public MethodCallAnalyzer(AnalysisAggregateSource source) : base(source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            AnalysisSource = source;
        }

        public bool WasMapBuilded { get; private set; }

        private void BuildDependencyMap()
        {
            if (WasMapBuilded) return;



            WasMapBuilded = true;
        }
    }
}

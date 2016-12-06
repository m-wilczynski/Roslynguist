namespace Roslynguist.Analyzers.MethodAnalyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Models;

    public class MethodCallAnalyzer : SyntaxAnalyzer
    {
        private readonly ConcurrentDictionary<IMethodSymbol, MethodModel> _methods = 
            new ConcurrentDictionary<IMethodSymbol, MethodModel>();
        private readonly ConcurrentDictionary<IFieldSymbol, FieldDeclarationSyntax> _fields =
            new ConcurrentDictionary<IFieldSymbol, FieldDeclarationSyntax>();
        private readonly ConcurrentDictionary<IPropertySymbol,PropertyDeclarationSyntax> _properties = 
            new ConcurrentDictionary<IPropertySymbol, PropertyDeclarationSyntax>();

        public readonly AnalysisAggregateSource AnalysisSource;

        public MethodCallAnalyzer(AnalysisAggregateSource source) : base(source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            AnalysisSource = source;
        }

        public bool WasMapBuilded { get; private set; }

        public void BuildDependencyMap()
        {
            if (WasMapBuilded) return;

            foreach (var compilation in AnalysisSource.GetCompilationsPerSolution())
            {
                var semanticModels =
                    compilation.Value.Select(cp => cp.SyntaxTrees.Select(st => 
                    new SemanticModelWithDescendants
                    {
                        SemanticModel = cp.GetSemanticModel(st),
                        TreeDescendants = st.GetRoot().DescendantNodes()
                    }))
                    .SelectMany(sm => sm).ToList();

                foreach (var model in semanticModels)
                {
                    foreach (var descendant in model.TreeDescendants)
                    {
                        AddIfMethod(descendant, model);
                        AddIfProperty(descendant, model);
                        AddIfField(descendant, model);
                    }
                }

            }

            WasMapBuilded = true;
        }

        private void AddIfField(SyntaxNode descendant, SemanticModelWithDescendants model)
        {
            if (descendant is FieldDeclarationSyntax)
            {
                foreach (var field in ((FieldDeclarationSyntax) descendant).Declaration.Variables)
                {
                    _fields.TryAdd(
                        (IFieldSymbol) model.SemanticModel.GetDeclaredSymbol(field),
                        (FieldDeclarationSyntax) descendant);
                }
            }
        }

        private void AddIfProperty(SyntaxNode descendant, SemanticModelWithDescendants model)
        {
            if (descendant is PropertyDeclarationSyntax)
            {
                _properties.TryAdd(
                    (IPropertySymbol) model.SemanticModel.GetDeclaredSymbol(descendant),
                    (PropertyDeclarationSyntax) descendant);
            }
        }

        private void AddIfMethod(SyntaxNode descendant, SemanticModelWithDescendants model)
        {
            if (descendant is MethodDeclarationSyntax)
            {
                IMethodSymbol symbol = (IMethodSymbol) model.SemanticModel.GetDeclaredSymbol(descendant);
                _methods.TryAdd(symbol, new MethodModel(symbol, (MethodDeclarationSyntax) descendant));
            }
        }
    }
}

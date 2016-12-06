namespace Roslynguist
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Models;
    using System.Linq;

    public class MembersCallMap
    {
        public readonly Dictionary<ISymbol, List<MemberDeclarationModel>> _members =
            new Dictionary<ISymbol, List<MemberDeclarationModel>>();

        public readonly AnalysisAggregateSource MapSource;

        public MembersCallMap(AnalysisAggregateSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            MapSource = source;
        }


        public bool WasMapBuilded { get; private set; }

        public void BuildDependencyMap()
        {
            if (WasMapBuilded) return;

            List<InvocationWithSemanticModel> _invocations = new List<InvocationWithSemanticModel>();

            foreach (var compilation in MapSource.GetCompilationsPerSolution())
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
                        AddIfMember(descendant, model);

                        if (descendant is InvocationExpressionSyntax)
                        {
                            _invocations.Add(new InvocationWithSemanticModel
                            {
                                Invocation = descendant as InvocationExpressionSyntax,
                                SemanticModel = model.SemanticModel
                            });
                        }
                    }
                }
            }

            foreach (var invocation in _invocations)
            {
                WireInvocation(invocation);
            }

            WasMapBuilded = true;
        }

        private void WireInvocation(InvocationWithSemanticModel invocation)
        {
            if (invocation != null)
            {
                var symbol = invocation.SemanticModel.GetSymbolInfo(invocation.Invocation.Expression).Symbol;
                if (symbol == null) return;

                List<MemberDeclarationModel> calleeParts;
                _members.TryGetValue(symbol, out calleeParts);

                if (calleeParts != null && calleeParts.Any())
                {
                    var caller = invocation.Invocation.FirstAncestorOrSelf<SyntaxNode>(sn =>
                        sn is MemberDeclarationSyntax);
                    if (caller != null)
                    {
                        var parentSymbol = invocation.SemanticModel.GetDeclaredSymbol(caller);
                        if (parentSymbol != null)
                        {
                            foreach (var calleePart in calleeParts)
                            {
                                calleePart.AddCaller(parentSymbol);
                            }
                            foreach (var callerPart in _members[parentSymbol])
                            {
                                callerPart.AddCallee(symbol);
                            }
                        }
                    }
                }
            }
        }

        private void AddIfMember(SyntaxNode descendant, SemanticModelWithDescendants model)
        {
            if (descendant is MemberDeclarationSyntax && !(descendant is NamespaceDeclarationSyntax))
            {
                ISymbol symbol = model.SemanticModel.GetDeclaredSymbol(descendant);
                if (symbol == null) return;

                //Support for partials
                if (!_members.ContainsKey(symbol))
                    _members.Add(symbol, new List<MemberDeclarationModel>());

                _members[symbol].Add(new MemberDeclarationModel(symbol, (MemberDeclarationSyntax)descendant));
            }
        }
    }
}

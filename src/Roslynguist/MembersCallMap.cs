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

            List<InvocationWithSemanticModel> invocations = new List<InvocationWithSemanticModel>();
            List<ObjectCreationWithSemanticModel> constructors = new List<ObjectCreationWithSemanticModel>();

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
                            invocations.Add(new InvocationWithSemanticModel
                            {
                                Expression = descendant as InvocationExpressionSyntax,
                                SemanticModel = model.SemanticModel
                            });
                        }
                        if (descendant is ObjectCreationExpressionSyntax)
                        {
                            constructors.Add(new ObjectCreationWithSemanticModel
                            {
                                Expression = descendant as ObjectCreationExpressionSyntax,
                                SemanticModel = model.SemanticModel
                            });
                        }
                    }
                }
            }

            foreach (var invocation in invocations)
            {
                var symbol = invocation.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol;
                if (symbol == null) continue;
                WireExpression(symbol, invocation.Expression, invocation.SemanticModel);
            }

            foreach (var ctor in constructors)
            {
                var symbol = ctor.SemanticModel.GetSymbolInfo(ctor.Expression).Symbol;
                if (symbol == null) continue;
                WireExpression(symbol, ctor.Expression, ctor.SemanticModel);
            }

            WasMapBuilded = true;
        }

        private void WireExpression(ISymbol expressionSymbol, ExpressionSyntax expression, SemanticModel semanticModel)
        {
            if (expression == null) return;

            List<MemberDeclarationModel> calleeParts;
            _members.TryGetValue(expressionSymbol, out calleeParts);

            if (calleeParts == null || !calleeParts.Any()) return;

            var caller = expression.FirstAncestorOrSelf<SyntaxNode>(sn =>
                sn is MemberDeclarationSyntax);

            if (caller == null) return;
            var parentSymbol = semanticModel.GetDeclaredSymbol(caller);
            if (parentSymbol == null) return;

            foreach (var calleePart in calleeParts)
            {
                calleePart.AddCaller(parentSymbol);
            }
            foreach (var callerPart in _members[parentSymbol])
            {
                callerPart.AddCallee(expressionSymbol);
            }
        }

        private void AddIfMember(SyntaxNode descendant, SemanticModelWithDescendants model)
        {
            if (!(descendant is MemberDeclarationSyntax) || descendant is NamespaceDeclarationSyntax) return;

            ISymbol symbol = model.SemanticModel.GetDeclaredSymbol(descendant);
            if (symbol == null) return;

            //Support for partials
            if (!_members.ContainsKey(symbol))
                _members.Add(symbol, new List<MemberDeclarationModel>());

            _members[symbol].Add(new MemberDeclarationModel(symbol, (MemberDeclarationSyntax)descendant));
        }
    }
}

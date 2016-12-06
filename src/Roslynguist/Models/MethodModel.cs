namespace Roslynguist.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodModel
    {
        public IMethodSymbol MethodSymbol;
        public readonly MethodDeclarationSyntax MethodSyntax;
        private readonly List<ISymbol> _callers = new List<ISymbol>();
        private readonly List<ISymbol> _callees = new List<ISymbol>();

        public MethodModel(IMethodSymbol methodSymbol, MethodDeclarationSyntax syntax)
        {
            MethodSymbol = methodSymbol;
            MethodSyntax = syntax;
        }

        public IReadOnlyCollection<ISymbol> MethodCallers => new ReadOnlyCollection<ISymbol>(_callers);
        public IReadOnlyCollection<ISymbol> MethodCallees => new ReadOnlyCollection<ISymbol>(_callees);

        public void AddCaller(ISymbol caller)
        {
            if (caller == null)
                throw new ArgumentNullException(nameof(caller));
            _callers.Add(caller);
        }

        public void AddCallee(ISymbol callee)
        {
            if (callee == null)
                throw new ArgumentNullException(nameof(callee));
            _callees.Add(callee);
        }
    }
}

namespace Roslynguist.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MemberDeclarationModel
    {
        public ISymbol MemberSymbol;
        public readonly MemberDeclarationSyntax MemberSyntax;
        private readonly List<ISymbol> _callers = new List<ISymbol>();
        private readonly List<ISymbol> _callees = new List<ISymbol>();

        public MemberDeclarationModel(ISymbol symbol, MemberDeclarationSyntax syntax)
        {
            MemberSymbol = symbol;
            MemberSyntax = syntax;
        }

        public IReadOnlyCollection<ISymbol> Callers => new ReadOnlyCollection<ISymbol>(_callers);
        public IReadOnlyCollection<ISymbol> Callees => new ReadOnlyCollection<ISymbol>(_callees);

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

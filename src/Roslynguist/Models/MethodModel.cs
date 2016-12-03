namespace Roslynguist.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;

    public class MethodModel
    {
        public readonly ISymbol OwnerClass;
        private readonly List<ISymbol> _callers = new List<ISymbol>();
        private readonly List<ISymbol> _callees = new List<ISymbol>();

        public MethodModel(ISymbol owner)
        {
            OwnerClass = owner;
        }

        public IReadOnlyCollection<ISymbol> MethodCallers => new ReadOnlyCollection<ISymbol>(_callers);
        public IReadOnlyCollection<ISymbol> MethodCallees => new ReadOnlyCollection<ISymbol>(_callees);

        public void AddCaller(ISymbol caller)
        {
            
        }

        public void AddCallee(ISymbol calee)
        {
            
        }
    }
}

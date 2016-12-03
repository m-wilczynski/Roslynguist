namespace Roslynguist.Selectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public abstract class SyntaxSelector
    {
        public readonly Guid Id;

        protected SyntaxSelector(Guid? id = null)
        {
            if (id.HasValue)
            {
                Id = id.Value;
                return;
            }
            Id = Guid.NewGuid();
        }

        protected abstract bool PerformCheck(SyntaxNode node);

        public IEnumerable<SyntaxNode> Apply(IEnumerable<SyntaxNode> nodes)
        {
            return nodes.Where(PerformCheck);
        }
    }
}

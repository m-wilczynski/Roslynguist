namespace Roslynguist.Analyzers
{
    using System;
    using System.Collections.Generic;
    using Selectors;

    public abstract class SyntaxAnalyzer
    {
        protected readonly Dictionary<Guid, SyntaxSelector> SyntaxSelectors = new Dictionary<Guid, SyntaxSelector>();
        protected readonly AnalysisAggregateSource AggregateSource;

        protected SyntaxAnalyzer(AnalysisAggregateSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            AggregateSource = source;
        }

        public void AddSelector(SyntaxSelector selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            SyntaxSelectors.Add(selector.Id, selector);
        }
    }
}
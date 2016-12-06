namespace Roslynguist.Models
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    public class SemanticModelWithDescendants
    {
        public SemanticModel SemanticModel { get; set; }
        public IEnumerable<SyntaxNode> TreeDescendants { get; set; }
    }
}

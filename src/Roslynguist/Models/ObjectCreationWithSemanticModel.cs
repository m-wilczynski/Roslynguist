namespace Roslynguist.Models
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ObjectCreationWithSemanticModel
    {
        public ObjectCreationExpressionSyntax Expression { get; set; }
        public SemanticModel SemanticModel { get; set; }
    }
}

namespace Roslynguist.Models
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class InvocationWithSemanticModel
    {
        public InvocationExpressionSyntax Invocation { get; set; }
        public SemanticModel SemanticModel { get; set; }
    }
}

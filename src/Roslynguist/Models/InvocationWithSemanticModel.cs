﻿namespace Roslynguist.Models
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class InvocationWithSemanticModel
    {
        public InvocationExpressionSyntax Expression { get; set; }
        public SemanticModel SemanticModel { get; set; }
    }
}

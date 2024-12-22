using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;


namespace ClassConstructorGenerator;


public class ClassConstructorGeneratorSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.ChildTokens().Any(y => y.IsKind(SyntaxKind.PartialKeyword))
            && !classDeclarationSyntax.ChildTokens().Any(y => y.IsKind(SyntaxKind.StaticKeyword))
            && !classDeclarationSyntax.ChildTokens().Any(y => y.IsKind(SyntaxKind.AbstractKeyword))
            && !classDeclarationSyntax.AttributeLists.Any(z => z.Attributes.Any(a => a.Name.ToString().Contains("IgnoreConstructorGenerator"))))
        {
            CandidateClasses.Add(classDeclarationSyntax);
        }
    }
}

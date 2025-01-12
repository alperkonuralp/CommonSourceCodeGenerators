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
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax) return;

        var tokens = classDeclarationSyntax.ChildTokens().ToList();

        if (!tokens.Any(y => y.IsKind(SyntaxKind.PartialKeyword))
            || tokens.Any(y => y.IsKind(SyntaxKind.StaticKeyword) || y.IsKind(SyntaxKind.AbstractKeyword))
            || HasIgnoreAttribute(classDeclarationSyntax))
        {
            return;
        }

        CandidateClasses.Add(classDeclarationSyntax);
    }

    private static bool HasIgnoreAttribute(ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AttributeLists.Any(z =>
            z.Attributes.Any(a => a.Name.ToString().Contains("IgnoreConstructorGenerator")));
}
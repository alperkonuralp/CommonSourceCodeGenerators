using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace ExportInterfaceGenerator;

public class ExportInterfaceSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclaration
            && classDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword) || x.IsKind(SyntaxKind.InternalKeyword))
            && !classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword)
            && classDeclaration.AttributeLists.All(x => x.Attributes.All(y => y.Name.ToString() != "IgnoreToInterfaceGeneration"))
            && classDeclaration.BaseList?.Types.Any(x => x.Type is not PredefinedTypeSyntax { Keyword: { Text: "object" } }) == true
        )
        {
            CandidateClasses.Add(classDeclaration);
        }
    }
}
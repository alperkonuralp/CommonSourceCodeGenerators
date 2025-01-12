using ExportInterfaceGenerator.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

namespace ExportInterfaceGenerator;

[Generator]
public class ExportInterfaceGenerator : ISourceGenerator
{
    private static int _runId;
    private GeneratorHeaderData _headerData;

    public void Initialize(GeneratorInitializationContext context)
    {
        Interlocked.Increment(ref _runId);
        context.RegisterForSyntaxNotifications(() => new ExportInterfaceSyntaxReceiver());
        _headerData = new GeneratorHeaderData
        {
            RunId = _runId
        };
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ExportInterfaceSyntaxReceiver receiver)
            return;

        _headerData.RootNamespace = context.Compilation.AssemblyName;
        _headerData.Compilation = context.Compilation.AssemblyName;

        context.AddSource("IgnoreToInterfaceGenerationAttribute.g.cs", IgnoreToInterfaceGenerationAttribute.Generate(_headerData, context.Compilation.AssemblyName));

        foreach (var classDeclaration in receiver.CandidateClasses.Distinct())
        {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
                continue;

            var interfaceName = $"I{classSymbol.Name}";

            var baseInterface = classDeclaration.BaseList.Types.Any(x => x.DescendantNodes().OfType<IdentifierNameSyntax>().Any(y => y.Identifier.Text == interfaceName));
            if (!baseInterface)
            {
                continue;
            }

            var interfaceNamespace = classSymbol.ContainingNamespace.ToDisplayString();
            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public
                    && m.GetAttributes().All(x => x.AttributeClass.Name != "IgnoreToInterfaceGenerationAttribute" && x.AttributeClass.Name != "IgnoreToInterfaceGeneration")
                    && m.Kind == SymbolKind.Property)
                .OrderBy(x => x.Name)
                .ToList();

            var methods = classSymbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public
                    && m.GetAttributes().All(x => x.AttributeClass.Name != "IgnoreToInterfaceGenerationAttribute" && x.AttributeClass.Name != "IgnoreToInterfaceGeneration")
                    && m.MethodKind == MethodKind.Ordinary
                    && m.Name != ".ctor"
                    && m.ContainingSymbol is not IPropertySymbol
                )
                .OrderBy(x => x.Name)
                .ToList();

            var source = GenerateInterface(interfaceNamespace, interfaceName, properties, methods);
            context.AddSource($"{interfaceName}.g.cs", source);
        }
    }

    private string GenerateInterface(string namespaceName, string interfaceName, List<IPropertySymbol> properties, List<IMethodSymbol> methods)
    {
        var members = properties
            .Select(PrepareProperty)
            .Concat(new[] { "" })
            .Concat(methods.Select(PrepareMethod))
            .ToList();

        return InterfaceTemplate.Generate(_headerData, namespaceName, interfaceName, members);
    }

    private static string PrepareMethod(IMethodSymbol method)
    {
        var xmlDocumentation = method.GetDocumentationCommentXml();
        var methodSignature = $"{method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(PrepareParameter))});";
        if (string.IsNullOrWhiteSpace(xmlDocumentation))
        {
            return "\n" + methodSignature;
        }
        else
        {
            var a = xmlDocumentation.Replace("\r\n", "\n").Split('\n').Where(x => !string.IsNullOrWhiteSpace(x) && x != "</member>").Skip(1).Select(x => "/// " + x.Trim());
            return $"\n{string.Join("\n", a)}\n{methodSignature}";
        }
    }

    private static string PrepareParameter(IParameterSymbol parameter)
    {
        if (parameter.IsParams)
        {
            return $"params {parameter.Type} {parameter.Name}";
        }
        switch (parameter.RefKind)
        {
            case RefKind.None:
                return $"{parameter.Type} {parameter.Name}";

            case RefKind.Ref:
                return $"ref {parameter.Type} {parameter.Name}";

            case RefKind.Out:
                return $"out {parameter.Type} {parameter.Name}";

            case RefKind.In: return $"in {parameter.Type} {parameter.Name}";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static string PrepareProperty(IPropertySymbol property)
    {
        var xmlDocumentation = property.GetDocumentationCommentXml();
        var methods = new List<string>(2);
        if (property.GetMethod != null && property.GetMethod.DeclaredAccessibility == Accessibility.Public)
            methods.Add("get;");
        if (property.SetMethod != null && property.SetMethod.DeclaredAccessibility == Accessibility.Public)
            methods.Add("set;");
        var result = $"\n{property.Type} {property.Name} {{ {string.Join(" ", methods)} }}";
        if (!string.IsNullOrWhiteSpace(xmlDocumentation))
        {
            var a = xmlDocumentation.Replace("\r\n", "\n").Split('\n').Where(x => !string.IsNullOrWhiteSpace(x) && x != "</member>").Skip(1).Select(x => "/// " + x.Trim());
            result = $"\n{string.Join("\n", a)}{result}";
        }
        return result;
    }
}
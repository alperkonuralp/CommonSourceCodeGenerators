using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClassConstructorGenerator;

[Generator]
public class ClassConstructorGenerator : ISourceGenerator
{
    private static int _runId;

    public void Initialize(GeneratorInitializationContext context)
    {
        Interlocked.Increment(ref _runId);
        context.RegisterForSyntaxNotifications(() => new ClassConstructorGeneratorSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Root namespace'i compilation'dan alabiliriz
        var rootNamespace = context.Compilation.AssemblyName;

        var attr = $@"using System;

namespace {rootNamespace}
{{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IgnoreConstructorGeneratorAttribute : Attribute {{ }}
}}";

        context.AddSource("IgnoreConstructorGeneratorAttribute.g.cs", attr);

        if (context.SyntaxReceiver is not ClassConstructorGeneratorSyntaxReceiver syntaxReceiver)
        {
            return;
        }

        foreach (var classSyntax in syntaxReceiver.CandidateClasses.Distinct())
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.

            if (model.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol symbol)
                continue;

            var classNamespaceName = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                                   .Replace("global::", "");

            var codes = new StringBuilder();

            codes.AppendLine($"namespace {classNamespaceName}");
            codes.AppendLine("{");

            var parentClass = symbol.BaseType;
            var parentClassConstructor = parentClass?.Constructors.FirstOrDefault(x => x.DeclaredAccessibility == Accessibility.Public);
            var parentClassConstructorParameters = (parentClassConstructor?.Parameters ?? Enumerable.Empty<IParameterSymbol>())
                .Select(x => new { x.Type, x.Name, x.DeclaredAccessibility, TypeName = x.GetType().Name, VariableName = x.Name.Trim('_') })
                .ToList();

            var fields = symbol.GetMembers()
                   .OfType<IFieldSymbol>()
                   .Where(x =>
                        x.DeclaredAccessibility == Accessibility.Private
                        && !x.IsConst
                        && x.IsReadOnly
                        && !x.IsStatic
                        && x.Name.StartsWith("_")
                        && !x.GetAttributes().Any(y => y.AttributeClass.Name == "IgnoreConstructorGeneratorAttribute"))
                   .Select(x => new { x.Type, x.Name, x.DeclaredAccessibility, TypeName = x.GetType().Name, VariableName = x.Name.Trim('_') })
                   .ToList();

            if (fields.Count > 0)
            {
                codes.AppendLine($"\tpublic partial class {symbol.Name}")
                    .AppendLine("\t{")
                    .AppendLine($"\t\tpublic {symbol.Name}(");

                var prefix = "";
                foreach (var (type, variableName) in parentClassConstructorParameters.Select(x => (x.Type, x.VariableName)).Union(fields.Select(x => (x.Type, x.VariableName))))
                {
                    codes.AppendLine($"\t\t\t{prefix}{type} {variableName}");
                    prefix = ", ";
                }

                codes.AppendLine("\t\t)");

                if (parentClass != null && parentClass.Name != "Object")
                {
                    codes.AppendLine("\t\t: base(" + string.Join(", ", parentClassConstructorParameters.Select(x => x.Name)) + ")");
                }

                codes.AppendLine("\t\t{");

                foreach (var field in fields.Select(x => $"\t\t\tthis.{x.Name} = {x.VariableName};"))
                {
                    codes.AppendLine(field);
                }

                codes.AppendLine("\t\t}");

                foreach (var field in fields.Select(x => $"\t\t// {x.TypeName} {x.Name} {x.DeclaredAccessibility}"))
                {
                    codes.AppendLine(field);
                }

                codes.AppendLine("\t}");
            }
            codes.AppendLine("}");

            context.AddSource($"{symbol.Name}.CCG.g.cs", codes.ToString());
        }
    }
}
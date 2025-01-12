using ClassConstructorGenerator.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ClassConstructorGenerator;

[Generator]
public class ClassConstructorGenerator : ISourceGenerator
{
    private static int _runId;
    private GeneratorHeaderData _headerData;
    private static readonly Accessibility[] ValidAccessibilities = new[] { Accessibility.Public, Accessibility.Protected, Accessibility.Internal, Accessibility.ProtectedOrInternal, Accessibility.ProtectedAndInternal };

    public void Initialize(GeneratorInitializationContext context)
    {
        Interlocked.Increment(ref _runId);
        context.RegisterForSyntaxNotifications(() => new ClassConstructorGeneratorSyntaxReceiver());
        _headerData = new GeneratorHeaderData
        {
            RunId = _runId
        };
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Root namespace'i compilation'dan alabiliriz
        var rootNamespace = context.Compilation.AssemblyName;

        _headerData.RootNamespace = context.Compilation.AssemblyName;
        _headerData.Compilation = context.Compilation.AssemblyName;

        context.AddSource("IgnoreConstructorGeneratorAttribute.g.cs", IgnoreConstructorGeneratorAttributeTemplate.Generate(_headerData, rootNamespace));

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

            var parentClass = symbol.BaseType;
            var parentClassConstructor = parentClass?.Constructors.Where(x => ValidAccessibilities.Contains(x.DeclaredAccessibility))
                                                                  .OrderByDescending(x => x.Parameters.Length)
                                                                  .FirstOrDefault();
            var parentClassConstructorParameters = (parentClassConstructor?.Parameters ?? Enumerable.Empty<IParameterSymbol>())
                .Select(x => new { x.Type, x.Name, x.DeclaredAccessibility, TypeName = x.Type.Name, VariableName = x.Name.Trim('_') })
                .ToList();

            var fields = symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(IsValidField)
                .Select(x => new { x.Type, x.Name, x.DeclaredAccessibility, TypeName = x.Type.Name, VariableName = x.Name.Trim('_') })
                .ToList();

            if (fields.Count > 0)
            {
                var classInfo = new GeneratedClassInfo
                {
                    ClassName = symbol.Name,
                    Namespace = classNamespaceName,
                    Accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant(),
                    Fields = fields.ConvertAll(f => new FieldInfo
                    {
                        Type = f.Type,
                        Name = f.Name,
                        DeclaredAccessibility = f.DeclaredAccessibility,
                        TypeName = f.TypeName,
                        VariableName = f.VariableName
                    }),
                    ParentConstructorParameters = parentClassConstructorParameters.ConvertAll(p => new ParameterInfo
                    {
                        Type = p.Type,
                        Name = p.Name,
                        DeclaredAccessibility = p.DeclaredAccessibility,
                        TypeName = p.TypeName,
                        VariableName = p.VariableName
                    })
                };

                var codes = GenerateClassCode(classInfo, context);
                context.AddSource($"{symbol.Name}.CCG.g.cs", codes);
            }
        }
    }

    private static bool IsValidField(IFieldSymbol field) =>
        field.DeclaredAccessibility == Accessibility.Private
        && !field.IsConst
        && field.IsReadOnly
        && !field.IsStatic
        && field.Name.StartsWith("_")
        && !field.GetAttributes().Any(y => y.AttributeClass.Name == "IgnoreConstructorGeneratorAttribute");

    private class FieldInfo
    {
        public ITypeSymbol Type { get; set; }
        public string Name { get; set; }
        public Accessibility DeclaredAccessibility { get; set; }
        public string TypeName { get; set; }
        public string VariableName { get; set; }
    }

    private class ParameterInfo
    {
        public ITypeSymbol Type { get; set; }
        public string Name { get; set; }
        public Accessibility DeclaredAccessibility { get; set; }
        public string TypeName { get; set; }
        public string VariableName { get; set; }
    }

    private class GeneratedClassInfo
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public List<FieldInfo> Fields { get; set; }
        public List<ParameterInfo> ParentConstructorParameters { get; set; }
        public string Accessibility { get; internal set; }
    }

    private string GenerateClassCode(GeneratedClassInfo classInfo, GeneratorExecutionContext context)
    {
        return ClassConstructorGeneratorTemplate.Generate(_headerData,
            classInfo.Namespace,
            classInfo.Accessibility,
            classInfo.ClassName,
            classInfo.ParentConstructorParameters.Select(x => (x.Type, x.VariableName)).Union(classInfo.Fields.Select(x => (x.Type, x.VariableName))).Select(x => new ParameterData { Type = x.Type.ToDisplayString(), VariableName = x.VariableName }).ToList(),
            classInfo.ParentConstructorParameters.Count == 0 ? null : string.Join(", ", classInfo.ParentConstructorParameters.Select(x => x.VariableName)),
            classInfo.Fields.ConvertAll(x => new VariableData { Name = x.Name, VariableName = x.VariableName }));
    }

    internal class ParameterData
    {
        public string Type { get; set; }
        public string VariableName { get; set; }
    }

    internal class VariableData
    {
        public string Name { get; set; }
        public string VariableName { get; set; }
    }
}
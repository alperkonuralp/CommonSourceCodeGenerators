using Microsoft.CodeAnalysis;
using Scriban;
using System.Collections.Generic;
using static ClassConstructorGenerator.ClassConstructorGenerator;

namespace ClassConstructorGenerator.Templates;

internal static class ClassConstructorGeneratorTemplate
{
    internal static string Generate(GeneratorHeaderData headerData, string namespaceName, string accessibility,
                                    string className, List<ParameterData> parameters,
                                    string baseClassVariables, List<VariableData> variables)
    {
        var templateContent = Resources.GetEmbeddedResourceContent("ClassConstructorGenerator.Templates.ClassConstructorGenerator.scriban");
        var template = Template.Parse(templateContent ?? "");
        return template.Render(new
        {
            HeaderData = headerData,
            Namespace = namespaceName,
            Accessibility = accessibility,
            ClassName = className,
            Parameters = parameters,
            BaseClassVariables = baseClassVariables,
            Variables = variables
        });
    }
}
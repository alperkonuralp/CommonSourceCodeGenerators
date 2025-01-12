using Scriban;

namespace ClassConstructorGenerator.Templates;

internal static class IgnoreConstructorGeneratorAttributeTemplate
{
    internal static string Generate(GeneratorHeaderData headerData, string namespaceName)
    {
        var templateContent = Resources.GetEmbeddedResourceContent("ClassConstructorGenerator.Templates.IgnoreConstructorGeneratorAttribute.scriban");
        var template = Template.Parse(templateContent ?? "");
        return template.Render(new
        {
            header_data = headerData,
            root_namespace = namespaceName
        });
    }
}
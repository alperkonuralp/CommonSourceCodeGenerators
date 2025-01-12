using Scriban;

namespace ExportInterfaceGenerator.Templates;

internal static class IgnoreToInterfaceGenerationAttribute
{
    internal static string Generate(GeneratorHeaderData headerData, string namespaceName)
    {
        var templateContent = Resources.GetEmbeddedResourceContent("ExportInterfaceGenerator.Templates.IgnoreToInterfaceGenerationAttribute.scriban");
        var template = Template.Parse(templateContent ?? "");
        return template.Render(new
        {
            header_data = headerData,
            @namespace = namespaceName
        });
    }
}
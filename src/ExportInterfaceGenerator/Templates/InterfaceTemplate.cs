using Scriban;

namespace ExportInterfaceGenerator.Templates;

internal static class InterfaceTemplate
{
    internal static string Generate(GeneratorHeaderData headerData, string namespaceName, string interfaceName, System.Collections.Generic.List<string> members)
    {
        var templateContent = Resources.GetEmbeddedResourceContent("ExportInterfaceGenerator.Templates.InterfaceTemplate.scriban");
        var template = Template.Parse(templateContent ?? "");
        return template.Render(new
        {
            header_data = headerData,
            @namespace = namespaceName,
            interface_name = interfaceName,
            members
        });
    }
}
using System.IO;
using System.Reflection;

namespace ExportInterfaceGenerator.Templates;

internal static class Resources
{
    internal static string GetEmbeddedResourceContent(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException("Resource not found", resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
using System;

namespace ExportInterfaceGenerator;

public class GeneratorHeaderData
{
    public string GeneratorVersion { get; set; } = typeof(GeneratorHeaderData).Assembly.GetName().Version.ToString();
    public string RootNamespace { get; set; }
    public string Compilation { get; set; }
    public DateTimeOffset CompileTime { get; set; } = System.DateTimeOffset.Now;
    public int RunId { get; set; }
}
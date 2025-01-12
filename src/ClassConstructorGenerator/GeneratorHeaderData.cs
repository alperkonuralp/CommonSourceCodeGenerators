using System;

namespace ClassConstructorGenerator;

public class GeneratorHeaderData
{
    public string GeneratorVersion { get; set; } = typeof(GeneratorHeaderData).Assembly.GetName().Version.ToString();
    public string RootNamespace { get; set; }
    public string Compilation { get; set; }
    public DateTimeOffset CompileTime { get; set; } = DateTimeOffset.Now;
    public int RunId { get; set; }
}
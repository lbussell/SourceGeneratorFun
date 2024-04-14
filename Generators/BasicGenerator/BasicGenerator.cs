using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BasicGenerator;

[Generator]
public class BasicGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
                postInitializationContext.AddSource(
                    $"{nameof(BasicGenerator)}.g.cs",
                    SourceText.From(GeneratedCode, Encoding.UTF8)));
    }

    public const string GeneratedCode = """
        using System;

        namespace GeneratedNamespace;

        public class Test
        {
            public void SayHello() => Console.WriteLine("Hello from generated code!");
        }

        """;
}

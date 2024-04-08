using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BasicGenerator;

[Generator]
public class BasicGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext => {
            postInitializationContext.AddSource("PostInit.g.cs", SourceText.From("""
                using System;
                namespace GeneratedNamespace;
                internal sealed class GeneratedAttribute : Attribute { }
                public class Test
                {
                    public void SayHello() => Console.WriteLine("Hello from generated code!");
                }
                """, Encoding.UTF8));
            });
    }
}

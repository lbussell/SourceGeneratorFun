using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGeneratorFun.Builders;

namespace SourceGeneratorFun.Generators;

[Generator]
public class FileTransformGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
                postInitializationContext.AddSource(
                    $"{nameof(FileTransformGenerator)}.g.cs",
                    SourceText.From(CodeGenerator.GeneratedInterface, Encoding.UTF8)));

        // var pipeline = context.AdditionalTextsProvider
        //     .Where(static (text) => text.Path.EndsWith(".json"))
        //     .Select(static (text, cancellationToken) =>
        //     {
        //         string name = Path.GetFileName(text.Path);

        //         SourceText additionalText = text.GetText(cancellationToken)
        //             ?? throw new InvalidOperationException($"There was an error reading from {text.Path}");

        //         var code = MyJsonToCSharpCompiler.Compile(additionalText);
        //         return (name, code);
        //     });

        // context.RegisterSourceOutput(pipeline,
        //     static (context, pair) =>
        //         context.AddSource($"{pair.name}.g.cs", SourceText.From(pair.code, Encoding.UTF8)));
    }
}

// internal record Thing(string Name, string Says);

internal static class CodeGenerator
{
    // public static string GeneratedNamespace = new CsharpBuilder()
    //     .AppendLine("namespace FileTransformGenerator;")
    //     .Build();

    public static string GeneratedNamespace = nameof(FileTransformGenerator);

    public static string GeneratedInterface = new CsharpBuilder()
        .WithNamespace(GeneratedNamespace)
        .NewLine()
        .OpenScope("public interface SaysHello")
            .AppendLine("void SayHello();")
        .CloseScope()
        .Build();
}

// internal sealed class MyJsonToCSharpCompiler
// {
//     public static string Compile(SourceText json)
//     {
//         // return json.;
//     }

//     private static string Compile(string json)
//     {
//         return "";
//     }
// }

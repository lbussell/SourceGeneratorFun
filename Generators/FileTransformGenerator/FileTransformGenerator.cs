using System.Text;
using System.Text.Json;
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

        var pipeline = context.AdditionalTextsProvider
            .Where(static (text) => text.Path.EndsWith(".json"))
            .SelectMany(GetClassCreationInfos)
            .Select(CodeGenerator.GenerateClassDefinition);

        context.RegisterSourceOutput(pipeline, static (context, classDefinition) =>
            context.AddSource(
                $"{classDefinition.Name}.g.cs",
                SourceText.From(classDefinition.Text, Encoding.UTF8)));
    }

    // One-to-many mapping
    private static IEnumerable<ClassCreationInfo> GetClassCreationInfos(AdditionalText text, CancellationToken ct) =>
        JsonSerializer.Deserialize<ClassCreationInfo[]>(text.GetText(ct)?.ToString() ?? "") ?? [];
}

internal record ClassCreationInfo(string Name, string Says);

internal record ClassDefinition(string Name, string Text);

internal static class CodeGenerator
{
    public static string GeneratedNamespace = nameof(FileTransformGenerator);

    public static string GeneratedInterfaceName = "SaysHello";
    public static string GeneratedMethodSignature = "void SayHello()";
    public static string GeneratedInterface = new CsharpBuilder()
        .WithNamespace(GeneratedNamespace)
        .NewLine()
        .OpenScope($"public interface {GeneratedInterfaceName}")
            .AppendLine(GeneratedMethodSignature + ";")
        .CloseScope()
        .Build();

    public static ClassDefinition GenerateClassDefinition(ClassCreationInfo data, CancellationToken _) =>
        new(data.Name, new CsharpBuilder()
            .WithNamespace(GeneratedNamespace)
            .NewLine()
            .OpenScope($"public class {data.Name} : {GeneratedInterfaceName}")
                .AppendLine($"public {GeneratedMethodSignature} => Console.WriteLine(\"{data.Says}\");")
            .CloseScope()
            .Build());
}

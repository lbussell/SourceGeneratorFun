using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceGeneratorFun.Builders;

namespace SourceGeneratorFun.Generators;

using MethodGenerationData = (MethodDefinition MethodToGenerate, ImmutableArray<ClassDefinition> ClassDefinitions);

[Generator]
public class FileTransformGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext =>
            postInitializationContext.AddSource(
                $"{nameof(FileTransformGenerator)}.g.cs",
                CodeGenerator.GeneratedInterface));

        var classDefinitionsPipeline = context.AdditionalTextsProvider
            .Where(static (text) => text.Path.EndsWith(".json"))
            .SelectMany(GetClassCreationInfos);

        context.RegisterSourceOutput(classDefinitionsPipeline, static (context, classDefinition) =>
            context.AddSource(
                $"{classDefinition.Name}.g.cs",
                classDefinition.Generate(new CancellationToken())));

        var methodGenerationPipeline = classDefinitionsPipeline.Collect();

        var generatedAttributePipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: $"{nameof(FileTransformGenerator)}.GenerateAttribute",
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is BaseMethodDeclarationSyntax,
            transform: static (context, cancellationToken) =>
            {
                var containingClass = context.TargetSymbol.ContainingType;
                return new MethodDefinition(
                    Namespace: containingClass.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? "",
                    ClassName: containingClass.Name,
                    MethodName: context.TargetSymbol.Name);
            });

        var finalMethodPipeline = generatedAttributePipeline.Combine(methodGenerationPipeline);

        context.RegisterSourceOutput<MethodGenerationData>(
            finalMethodPipeline,
            static (context, data) => context.AddSource(
                data.MethodToGenerate.ClassName + ".g.cs",
                CodeGenerator.CallAllGeneratedMethods(data.MethodToGenerate, data.ClassDefinitions)));
    }

    // One-to-many mapping
    private static IEnumerable<ClassDefinition> GetClassCreationInfos(AdditionalText text, CancellationToken ct) =>
        JsonSerializer.Deserialize<ClassDefinition[]>(text.GetText(ct)?.ToString() ?? "") ?? [];
}

internal record ClassDefinition(string Name, string Says)
{
    public SourceText Generate(CancellationToken ct) =>
        CodeGenerator.GenerateClassDefinition(this, ct);
};

internal record MethodDefinition(string Namespace, string ClassName, string MethodName);

internal static class CodeGenerator
{
    public static string GeneratedNamespace = nameof(FileTransformGenerator);

    public static string GeneratedInterfaceName = "SaysHello";

    public static string GeneratedMethodSignature = "void SayHello()";

    public static SourceText GeneratedInterface = GetNewBuilder()
        .AppendLine("using System;")
        .OpenScope($"public interface {GeneratedInterfaceName}")
            .AppendLine(GeneratedMethodSignature + ";")
        .CloseScope()
        .NewLine()
        .AppendLine("[AttributeUsage(AttributeTargets.Method)]")
        .AppendLine("internal sealed class GenerateAttribute : Attribute { }")
        .BuildSourceText();

    public static SourceText GenerateClassDefinition(ClassDefinition data, CancellationToken _) =>
        GetNewBuilder()
            .OpenScope($"public class {data.Name} : {GeneratedInterfaceName}")
                .AppendLine($"public {GeneratedMethodSignature} => Console.WriteLine(\"{data.Says}\");")
            .CloseScope()
            .BuildSourceText();

    public static SourceText CallAllGeneratedMethods(
        MethodDefinition method,
        IEnumerable<ClassDefinition> classDefinitions)
        {
            var builder = new CsharpBuilder()
                .WithNamespace(method.Namespace)
                .OpenScope($"partial class {method.ClassName}")
                .OpenScope($"static partial void {method.MethodName}()");

            foreach (var c in classDefinitions)
            {
                builder.AppendLine($"new {GeneratedNamespace}.{c.Name}().SayHello();");
            }

            return builder
                .CloseScope()
                .CloseScope()
                .BuildSourceText();
        }

    private static CsharpBuilder GetNewBuilder() =>
        new CsharpBuilder().WithNamespace(GeneratedNamespace).NewLine();

    private static SourceText BuildSourceText(this CsharpBuilder builder) =>
        SourceText.From(builder.Build(), Encoding.UTF8);
}

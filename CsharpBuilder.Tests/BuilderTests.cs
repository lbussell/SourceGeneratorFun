namespace SourceGeneratorFun.CsharpBuilder.Tests;

using SourceGeneratorFun.Builders;

public class BuilderTests
{
    [Fact]
    public void VerifyBasicOutput()
    {
        CsharpBuilder builder = new CsharpBuilder()
            .WithNamespace("TestNamespace")
            .NewLine()
            .OpenScope("public class Person")
                .OpenScope("public void SayHello()")
                    .AppendLine("Console.WriteLine(\"Hello World!\");")
                .CloseScope()
            .CloseScope();

        Assert.Equal(
            NormalizeNewlines(Expected),
            NormalizeNewlines(builder.Build()));
    }

    private static readonly string Expected = """
        namespace TestNamespace;

        public class Person
        {
            public void SayHello()
            {
                Console.WriteLine("Hello World!");
            }
        }

        """;

    private static string NormalizeNewlines(string input) => input.Replace("\r\n", "\n");
}

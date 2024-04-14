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

        Assert.Equal(Expected, builder.Build());
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
}

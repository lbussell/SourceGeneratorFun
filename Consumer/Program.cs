namespace SourceGeneratorFun.Consumer;

using GeneratedNamespace;

partial class Program
{
    static void Main(string[] args)
    {
        // BasicGenerator
        TestBasicGenerator();

        // FileTransformGenerator
        TestFileTransformGenerator();
    }

    static void TestBasicGenerator()
    {
        var test = new Test();
        test.SayHello();
    }

    [FileTransformGenerator.Generate]
    static partial void TestFileTransformGenerator();
}

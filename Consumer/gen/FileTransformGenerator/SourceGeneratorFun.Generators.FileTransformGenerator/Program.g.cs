namespace SourceGeneratorFun.Consumer;
partial class Program
{
    static partial void TestFileTransformGenerator()
    {
        new FileTransformGenerator.Cat().SayHello();
        new FileTransformGenerator.Dog().SayHello();
        new FileTransformGenerator.Human().SayHello();
        new FileTransformGenerator.Iguana().SayHello();
        new FileTransformGenerator.Cowboy().SayHello();
    }
}

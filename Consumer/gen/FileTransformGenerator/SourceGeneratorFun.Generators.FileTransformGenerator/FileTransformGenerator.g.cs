namespace FileTransformGenerator;

using System;
public interface SaysHello
{
    void SayHello();
}

[AttributeUsage(AttributeTargets.Method)]
internal sealed class GenerateAttribute : Attribute { }

namespace SourceGeneratorFun.Builders;

using System.Text;

public sealed class CsharpBuilder(int Spaces = 4)
{
    private readonly StringBuilder _builder = new();

    private int _indentationLevel = 0;

    private string Indentation => new(' ', _indentationLevel * Spaces);

    private string GetIndentation(int level) => new(' ', level * Spaces);

    public CsharpBuilder WithNamespace(string name)
    {
        _builder.Insert(0, $"namespace {name};" + Environment.NewLine);
        return this;
    }

    public CsharpBuilder OpenScope(string line = "")
    {
        AppendLine(line);
        AppendLine("{");
        IncreaseIndent();
        return this;
    }

    public CsharpBuilder CloseScope()
    {
        DecreaseIndent();
        AppendLine("}");
        return this;
    }

    public CsharpBuilder NewLine() => AppendLine();

    public CsharpBuilder AppendLine(string line = "")
    {
        _builder.AppendLine(Indentation + line);
        return this;
    }

    public CsharpBuilder IncreaseIndent()
    {
        _indentationLevel += 1;
        return this;
    }

    public CsharpBuilder DecreaseIndent()
    {
        _indentationLevel -= 1;
        return this;
    }

    public string Build() => _builder.ToString();
}

namespace Habari.Library.Parameters;

public interface IInput : IParameter
{
    bool IsLinked { get; }

    bool IsRequired { get; }

    IOutput? Source { get; }

    bool Link(IOutput source);
}

namespace Habari.Library.Parameters
{
    public interface IInputParameter : IParameter
    {
        bool IsLinked { get; }

        bool IsRequired { get; }

        IOutputParameter? Source { get; }

        bool Link(IOutputParameter source);
    }
}

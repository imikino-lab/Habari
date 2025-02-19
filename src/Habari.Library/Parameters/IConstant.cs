using Habari.Library.Steps;

namespace Habari.Library.Parameters;

public interface IConstant
{
    string Code { get; }

    string Name { get; }

    IRoot Step { get; }

    Type[] Types { get; }

    string ContextKey { get; }

    public T? GetValue<T>(WorkflowContext context);

    public void SetValue(WorkflowContext context, params (Type, object?)[] values);
}

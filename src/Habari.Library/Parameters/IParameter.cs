using Habari.Library.Json;
using Habari.Library.Steps;

namespace Habari.Library.Parameters;

[InterfaceJsonConverter(typeof(InterfaceJsonConverter<IParameter>))]
public interface IParameter
{
    string Code { get; }

    string Name { get; }

    IBase Step { get; }

    Type[] Types { get; }

    string ContextKey { get; }

    public T? GetValue<T>(WorkflowContext context);

    public void SetValue(WorkflowContext context, params (Type, object?)[] values);
}

using Habari.Library.Steps;
using System.Text.Json.Serialization;

namespace Habari.Library.Parameters;

public class Input : IInput
{
    public string Code { get; private set; }

    [JsonIgnore]
    public string ContextKey => $"{Source?.ContextKey}";

    [JsonIgnore]
    public bool IsLinked => Source != null;

    public bool IsRequired { get; private set; }

    public string Name { get; private set; }

    [JsonIgnore]
    public IOutput? Source { get; private set; }

    [JsonIgnore]
    public IBase Step { get; private set; }

    [JsonIgnore]
    public Type[] Types { get; private set; }

    public Input(IBase step, string code, string name, bool isRequired, params Type[] types)
    {
        Code = code.ToLower();
        IsRequired = isRequired;
        Name = name;
        Step = step;
        Types = types;
    }

    public T? GetValue<T>(WorkflowContext context)
    {
        return (T?)context.Get((Source?.Code ?? Code), typeof(T));
    }

    public bool Link(IOutput source)
    {
        bool result = false;
        if (source.Types.Any(type => Types.Contains(type)))
        {
            Source = source;
            result = true;
        }
        return result;
    }

    public void SetValue(WorkflowContext context, params (Type, object?)[] values)
    {
        context.Set(Code, values);
    }
}

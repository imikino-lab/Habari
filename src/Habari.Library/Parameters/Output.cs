﻿using Habari.Library.Steps;
using System.Text.Json.Serialization;

namespace Habari.Library.Parameters;

public class Output : IOutput
{
    public string Code { get; private set; }

    public string Name { get; private set; }

    [JsonIgnore]
    public IBase Step { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ParameterType Type { get; private set; }

    [JsonIgnore]
    public Type[] Types { get; private set; }

    [JsonIgnore]
    public string ContextKey => $"{Step.Code}.{Step.Id}.Output.{Code}";

    public Output(IBase step, string code, string name, ParameterType type, params Type[] types)
    {
        Code = code;
        Name = name;
        Step = step;
        Type = type;
        Types = types;
    }

    public T? GetValue<T>(WorkflowContext context)
    {
        return (T?)context.Get(Code, typeof(T));
    }

    public void SetValue(WorkflowContext context, params (Type, object?)[] values)
    {
        context.Set(Code, values);
    }
}

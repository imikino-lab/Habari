﻿using Habari.Library.Steps;
using System.Text.Json.Serialization;

namespace Habari.Library.Parameters;

public class Constant : IConstant
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
    public IRoot Step { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ParameterType Type { get; private set; }

    [JsonIgnore]
    public Type[] Types { get; private set; }

    public Constant(IRoot step, string code, string name, ParameterType type, bool isRequired, params Type[] types)
    {
        Code = code;
        IsRequired = isRequired;
        Name = name;
        Step = step;
        Type = type;
        Types = types;
    }

    public T? GetValue<T>(WorkflowContext context)
    {
        return (T?)context.Get((Source?.Code ?? Code), typeof(T));
    }

    public void SetValue(WorkflowContext context, params (Type, object?)[] values)
    {
        context.Set(Code, values);
    }
}

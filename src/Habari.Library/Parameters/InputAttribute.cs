namespace Habari.Library.Parameters;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class InputAttribute : Attribute
{
    public string Code { get; private set; }

    public bool IsRequired { get; private set; }

    public string Name { get; private set; }

    public ParameterType ParameterType { get; private set; }

    public Type[] Types { get; private set; }

    public InputAttribute(string code, string name, ParameterType parameterType, params Type[] types) : this(code, name, parameterType, false, types) { }

    public InputAttribute(string code, string name, ParameterType parameterType, bool isRequired, params Type[] types)
    {
        Code = code;
        IsRequired = isRequired;
        Name = name;
        ParameterType = parameterType;
        Types = types;
    }
}

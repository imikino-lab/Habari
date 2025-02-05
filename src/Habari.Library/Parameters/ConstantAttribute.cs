namespace Habari.Library.Parameters;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ConstantAttribute : Attribute
{
    public string Code { get; private set; }

    public bool IsRequired { get; private set; }

    public string Name { get; private set; }

    public ParameterType ParameterType { get; set; }

    public ConstantAttribute(string code, string name, ParameterType parameterType) : this(code, name, parameterType, false) { }

    public ConstantAttribute(string code, string name, ParameterType parameterType, bool isRequired)
    {
        Code = code;
        IsRequired = isRequired;
        Name = name;
        ParameterType = parameterType;
    }
}

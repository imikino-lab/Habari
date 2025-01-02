namespace Habari.Library.Parameters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ConstantAttribute : Attribute
    {
        public string Code { get; private set; }

        public bool IsRequired { get; private set; }

        public string Name { get; private set; }

        public ConstantType Type { get; set; }

        public ConstantAttribute(string code, string name, ConstantType type) : this(code, name, type, false) { }

        public ConstantAttribute(string code, string name, ConstantType type, bool isRequired)
        {
            Code = code;
            IsRequired = isRequired;
            Name = name;
            Type = type;
        }
    }
}

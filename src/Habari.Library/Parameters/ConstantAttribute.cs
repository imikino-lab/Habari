namespace Habari.Library.Parameters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ConstantAttribute : Attribute
    {
        public string Code { get; private set; }

        public bool IsRequired { get; private set; }

        public string Name { get; private set; }

        public ConstantAttribute(string code, string name) : this(code, name, false) { }

        public ConstantAttribute(string code, string name, bool isRequired)
        {
            Code = code;
            Name = name;
            IsRequired = isRequired;
        }
    }
}

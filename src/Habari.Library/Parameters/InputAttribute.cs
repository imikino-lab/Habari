using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InputAttribute : Attribute
    {
        public string Code { get; private set; }

        public bool IsRequired { get; private set; }

        public string Name { get; private set; }

        public Type[] Types { get; private set; }

        public InputAttribute(string code, string name, params Type[] types) : this(code, name, false, types) { }

        public InputAttribute(string code, string name, bool isRequired, params Type[] types)
        {
            Code = code;
            Name = name;
            IsRequired = isRequired;
            Types = types;
        }
    }
}

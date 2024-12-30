using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OutputAttribute : Attribute
    {
        public string Code { get; private set; }

        public string Name { get; private set; }

        public Type[] Types { get; private set; }

        public OutputAttribute(string code, string name, params Type[] types)
        {
            Code = code;
            Name = name;
            Types = types;
        }
    }
}

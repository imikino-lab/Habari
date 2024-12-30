using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    public class Constant
    {
        public string Code { get; private set; }

        public bool IsRequired { get; private set; }

        public string Name { get; private set; }

        public IStep Step { get; private set; }

        public Constant(IStep step, string code, string name, bool isRequired)
        {
            Code = code.ToLower();
            IsRequired = isRequired;
            Name = name;
            Step = step;
        }
    }
}

using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    public class Input : IInput
    {
        public string Code { get; private set; }

        public string ContextKey => $"{Source?.ContextKey}";

        public bool IsLinked => Source != null;

        public bool IsRequired { get; private set; }

        public string Name { get; private set; }

        public IOutput? Source { get; private set; }

        public IStep Step { get; private set; }

        public Type[] Types { get; private set; }

        public Input(IStep step, string code, string name, bool isRequired, params Type[] types)
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
}

using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    public class Output : IOutput
    {
        public string Code { get; private set; }

        public string Name { get; private set; }

        public IStep Step { get; private set; }

        public Type[] Types { get; private set; }

        public string ContextKey => $"{Step.Code}.{Step.Id}.Output.{Code}";

        public Output(IStep step, string code, string name, params Type[] types)
        {
            Code = code.ToLower();
            Name = name;
            Step = step;
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
}

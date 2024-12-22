using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    public interface IParameter
    {
        string Code { get; }

        string Name { get; }

        IStep Step { get; }

        Type[] Types { get; }

        string ContextKey { get; }

        public T? GetValue<T>(WorkflowContext context);

        public void SetValue(WorkflowContext context, params (Type, object?)[] values);
    }
}

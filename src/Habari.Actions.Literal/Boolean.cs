using Habari.Library.Parameters;
using Habari.Library.Steps;

namespace Habari.Actions.Literal;

public class Boolean : Step
{
    public override string Code => "Habari.Action.Literal.Boolean";

    [Output("content", "Boolean content", ParameterType.Boolean, typeof(bool))]
    public Output Content => Outputs["content"];

    public override string Description => "Return the content of the given boolean";

    public override string Name => "Literal boolean";

    [Constant("boolean", "Value", ParameterType.Boolean, true)]
    public Constant Value => Constants["boolean"];

    public override Task RunAsync(WorkflowContext context)
    {
        bool boolean = Value.GetValue<bool>(context)!;
        Content.SetValue(context, (typeof(bool), boolean));
        return Task.CompletedTask;
    }
}

using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text;

namespace Habari.Actions.Literal;

public class Text : Step
{
    public override string Code => "Habari.Action.Literal.Text";

    [Output("content", "Text content", ParameterType.Text, typeof(byte[]), typeof(string))]
    public Output Content => Outputs["content"];

    public override string Description => "Return the content of the given text";

    public override string Name => "Literal text";

    [Constant("text", "Value", ParameterType.Text, true)]
    public Constant Value => Constants["text"];

    public override Task RunAsync(WorkflowContext context)
    {
        string text = Value.GetValue<string>(context)!;
        byte[] textByte = Encoding.Default.GetBytes(text);
        Content.SetValue(context, (typeof(string), text), (typeof(byte[]), textByte));
        return Task.CompletedTask;
    }
}

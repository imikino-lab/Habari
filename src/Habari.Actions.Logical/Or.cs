using Habari.Library.Parameters;
using Habari.Library.Steps;

namespace Habari.Actions.Logical
{
    public class Or : Step
    {
        public override string Code => "Habari.Action.Logical.Or";

        public override string Description => "Perform a logical or operation";

        public override string Name => "Logical or";

        [Input("op1", "Operator 1", true, typeof(bool))]
        public Input Operator1 => Inputs["op1"];

        [Input("op2", "Operator 2", true, typeof(bool))]
        public Input Operator2 => Inputs["op2"];

        [Output("result", "Or operation result", typeof(bool))]
        public Output Result => Outputs["result"];

        public override Task RunAsync(WorkflowContext context)
        {
            bool op1 = Operator1.GetValue<bool>(context)!;
            bool op2 = Operator2.GetValue<bool>(context)!;
            Result.SetValue(context, (typeof(bool), op1 | op2));
            return Task.CompletedTask;
        }
    }
}

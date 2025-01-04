using Habari.Library.Parameters;
using Habari.Library.Steps;

namespace Habari.Actions.Logical
{
    public class Not : Step
    {
        public override string Code => "Habari.Action.Logical.Not";

        public override string Description => "Perform a logical not operation";

        public override string Name => "Logical not";

        [Input("op", "Operator", true, typeof(bool))]
        public Input Operator => Inputs["op"];

        [Output("result", "Not operation result", typeof(bool))]
        public Output Result => Outputs["result"];

        public override Task RunAsync(WorkflowContext context)
        {
            bool op = Operator.GetValue<bool>(context)!;
            Result.SetValue(context, (typeof(bool), !op));
            return Task.CompletedTask;
        }
    }
}

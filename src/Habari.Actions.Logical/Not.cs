using Habari.Library.Parameters;
using Habari.Library.Steps;

namespace Habari.Actions.Logical
{
    public class Not : Step
    {
        public override string Code => throw new NotImplementedException();

        public override string Description => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        [Input("op", "Operator", true, typeof(bool))]
        public Input Operator => Inputs["op"];

        [Output("result", "And operation result", typeof(bool))]
        public Output Result => Outputs["result"];

        public override Task RunAsync(WorkflowContext context)
        {
            bool op = Operator.GetValue<bool>(context)!;
            Result.SetValue(context, (typeof(bool), !op));
            return Task.CompletedTask;
        }
    }
}

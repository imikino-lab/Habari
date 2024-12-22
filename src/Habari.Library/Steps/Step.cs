using Habari.Library.Parameters;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps
{
    public abstract class Step : IStep
    {
        public int Id { get; private set; }

        public abstract string Code { get; }

        public abstract string Description { get; }

        public abstract string Name { get; }

        public InputParameters Inputs { get; protected set; } = new InputParameters();

        public OutputParameters Outputs { get; protected set; } = new OutputParameters();

        public StepStatus Status { get; protected set; }

        public void Load(JsonObject config)
        {
            Id = config["id"]!.GetValue<int>();
            LoadCustomParameters(config);
        }

        public abstract void LoadCustomParameters(JsonObject config);

        public abstract void Run(WorkflowContext context);
    }
}

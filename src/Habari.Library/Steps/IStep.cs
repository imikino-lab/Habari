using Habari.Library.Parameters;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps
{
    public interface IStep
    {
        int Id { get; }

        string Code { get; }

        string Description { get; }

        string Name { get; }

        InputParameters Inputs { get; }

        OutputParameters Outputs { get; }

        StepStatus Status { get; }

        void Load(JsonObject config);

        void LoadCustomParameters(JsonObject config);

        void Run(WorkflowContext context);
    }
}

using Habari.Library.Parameters;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps;

public interface IBase
{
    int Id { get; }

    string Code { get; }

    Constants Constants { get; }

    string Description { get; }

    Inputs Inputs { get; }

    string Name { get; }

    Outputs Outputs { get; }

    StepStatus Status { get; }

    float X { get; set; }

    float Y { get; set; }

    void Load(JsonObject config);

    Task RunAsync(WorkflowContext context);
}

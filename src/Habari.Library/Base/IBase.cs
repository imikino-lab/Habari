using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Base;

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

    int X { get; set; }

    int Y { get; set; }

    void Load(JsonObject config);

    Task RunAsync(WorkflowContext context);
}

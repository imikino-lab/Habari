using Habari.Library.Parameters;

namespace Habari.Library.Steps;

public interface IBase : IRoot
{
    int Id { get; }

    Inputs Inputs { get; }

    Outputs Outputs { get; }

    StepStatus Status { get; }

    Task RunAsync(WorkflowContext context);
}

using Habari.Library.Listeners;
using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Workflows;

public class Workflow
{
    public string? Code { get; set; }

    public string? Description { get; set; }

    public IListener? Listener { get; protected set; }

    public string? Name { get; set; }

    public WorkflowStatus Status { get; protected set; }

    public Workflow(IListener listener)
    {
        Listener = listener;
    }

    public void Execute(WorkflowContext context)
    {
        Listener!.Start(context);
    }

    public void Load(JsonObject config)
    {
        Listener?.Load(config);
        foreach (JsonNode? triggerConfig in config!["triggers"]!.AsArray())
        {
            Listener?.LoadTrigger(triggerConfig!.AsObject());
        }
    }
}

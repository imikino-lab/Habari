using Habari.Library.Listeners;
using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Workflows;

public class Workflow
{
    public string? Code { get; set; }

    public WorkflowContext Context { get; } = new ();

    public string? Description { get; set; }

    public IListener? Listener { get; protected set; }

    public string? Name { get; set; }

    public WorkflowStatus Status { get; protected set; }

    public Workflow(IListener listener)
    {
        Listener = listener;
    }

    public void Execute()
    {
        Listener!.Start(Context);
    }

    public void Load(JsonObject configuration)
    {
        Listener?.Load(configuration);
        Listener?.Triggers.Clear();
        foreach (JsonNode? triggerConfig in configuration!["triggers"]!.AsArray())
        {
            Listener?.LoadTrigger(triggerConfig!.AsObject());
        }
    }
}

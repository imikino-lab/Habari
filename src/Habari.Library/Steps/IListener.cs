using Habari.Library.Json;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps;

[InterfaceJsonConverter(typeof(InterfaceJsonConverter<IListener>))]
public interface IListener : IRoot
{
    List<ITrigger> Triggers { get; }

    void LoadTrigger(JsonObject config);

    void Start(WorkflowContext context);

    void Stop();
}

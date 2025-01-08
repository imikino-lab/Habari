using Habari.Library.Json;
using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Listeners;

[InterfaceJsonConverter(typeof(InterfaceJsonConverter<IListener>))]
public interface IListener
{
    string Code { get; }

    string Description { get; }

    string Name { get; }

    List<ITrigger> Triggers { get; }

    int X { get; set; }

    int Y { get; set; }

    Constants Values { get; }

    void Load(JsonObject config);

    void LoadTrigger(JsonObject config);

    void Start(WorkflowContext context);

    void Stop();
}

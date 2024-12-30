using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Listeners
{
    public interface IListener
    {
        string Code { get; }

        string Description { get; }

        string Name { get; }

        List<IStep> Triggers { get; }

        int X { get; set; }

        int Y { get; set; }

        Constants Values { get; }

        void Load(JsonObject config);

        void LoadTrigger(JsonObject config);

        void Start(WorkflowContext context);

        void Stop();
    }
}

using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Listeners
{
    public interface IListener
    {
        string Code { get; }

        string Description { get; }

        string Name { get; }

        IList<IStep> Triggers { get; }

        void Load(JsonObject config);

        void LoadTrigger(JsonObject config);

        void Start(WorkflowContext context);

        void Stop();
    }
}

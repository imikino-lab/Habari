using Habari.Library.Steps;
using System.Text.Json.Nodes;

namespace Habari.Library.Listeners
{
    public abstract class Listener : IListener
    {
        public abstract string Code { get; }

        public abstract string Description { get; }

        public abstract string Name { get; }

        public IList<IStep> Triggers { get; protected set; } = new List<IStep>();

        public abstract void Load(JsonObject config);

        public abstract void LoadTrigger(JsonObject config);

        public abstract void Start(WorkflowContext context);

        public abstract void Stop();
    }
}

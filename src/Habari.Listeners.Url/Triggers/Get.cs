using Habari.Library.Steps;
using System.Text.Json.Nodes;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Listeners.Url.Triggers
{
    public class Get : UrlTrigger
    {
        public override string Code => "Get";

        public override string Description => "Occurs when a url is asked with the REST method GET";

        public override string Name => "Url get asked";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public Get() : base() { }

        public override void HandleException(WorkflowContext context, Exception exception)
        {
            
        }

        public override void LoadCustomParameters(JsonObject config)
        {
            if(config["route"] != null)
                Route = config["route"]!.GetValue<string>();
        }
    }
}

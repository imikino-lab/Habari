using Habari.Library.Parameters;
using Habari.Library.Steps;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Listeners.Url.Triggers;

public abstract class UrlTrigger : Trigger
{
    [Output("context", "Web context", ParameterType.Context, typeof(HttpContextBase))]
    public Output Context => Outputs["context"];

    public abstract HttpMethod HttpMethod { get; }

    public string Route { get; set; } = "/";

    [Output("path", "Asked path", ParameterType.Path, typeof(byte[]), typeof(string))]
    public Output Path => Outputs["path"];
}

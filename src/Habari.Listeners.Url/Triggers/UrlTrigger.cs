using Habari.Library.Parameters;
using Habari.Library.Steps;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Listeners.Url.Triggers;

public abstract class UrlTrigger : Trigger
{
    public InputParameter Content => Inputs["content"];

    public InputParameter ContentNotFound => Inputs["contentNotFound"];

    public abstract HttpMethod HttpMethod { get; }

    public string Route { get; set; } = "/";

    public OutputParameter Path => Outputs["path"];

    public UrlTrigger()
    {
        Inputs.Add(new InputParameter(this, "content", "File content", true, typeof(byte[]), typeof(string)));
        Inputs.Add(new InputParameter(this, "contentNotFound", "Content not found", true, typeof(bool)));
        Outputs.Add(new OutputParameter(this, "path", "Asked path", typeof(byte[]), typeof(string)));
    }
}

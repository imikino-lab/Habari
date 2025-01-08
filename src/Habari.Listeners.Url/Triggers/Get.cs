using Habari.Library.Steps;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Listeners.Url.Triggers;

public class Get : UrlTrigger
{
    public override string Code => "Get";

    public override string Description => "Occurs when a url is asked with the REST method GET";

    public override string Name => "Url get asked";

    public override HttpMethod HttpMethod => HttpMethod.GET;

    public override void HandleException(WorkflowContext context, Exception exception)
    {
        
    }
}

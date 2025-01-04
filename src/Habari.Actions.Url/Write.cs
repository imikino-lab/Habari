using Habari.Library.Parameters;
using Habari.Library.Steps;
using WatsonWebserver.Core;

namespace Habari.Actions.Url
{
    public class Write : Step
    {
        public override string Code => "Habari.Action.Url.Write";

        [Input("content", "File content", true, typeof(byte[]), typeof(string))]
        public Input Content => Inputs["content"];

        [Input("contentNotFound", "Content not found", true, typeof(bool))]
        public Input ContentNotFound => Inputs["contentNotFound"];

        [Input("context", "Current web context", true, typeof(HttpContextBase))]
        public Input Context => Inputs["context"];

        public override string Description => "Write the content of a file";
        
        public override string Name => "Url writer";
        
        public override async Task RunAsync(WorkflowContext context)
        {
            HttpContextBase? contextBase = Context.GetValue<HttpContextBase>(context);
            if (ContentNotFound.GetValue<bool>(context))
            {
                contextBase!.Response.StatusCode = 404;
            }
            await contextBase!.Response.Send(Content.GetValue<byte[]>(context));
        }
    }
}

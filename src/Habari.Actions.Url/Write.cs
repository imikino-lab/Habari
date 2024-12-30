using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text.Json.Nodes;
using WatsonWebserver.Core;

namespace Habari.Actions.Url
{
    public class Write : Step
    {
        public override string Code => "Habari.Action.Url.Write";

        public Input Content => Inputs["content"];

        public Input ContentNotFound => Inputs["contentNotFound"];

        public Input Context => Inputs["context"];

        public override string Description => "Write the content of a file";
        
        public override string Name => "";
        
        public Write()
        {
            Inputs.Add(new Input(this, "content", "File content", true, typeof(byte[]), typeof(string)));
            Inputs.Add(new Input(this, "contentNotFound", "Content not found", true, typeof(bool)));
            Inputs.Add(new Input(this, "context", "Current web context", true, typeof(HttpContextBase)));
        }

        /*
        public override void LoadConstants(JsonObject config)
        {
        }
        */
        
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

using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Text;
using System.Text.Json.Nodes;
using IO = System.IO;

namespace Habari.Actions.File
{
    public class Read : Step
    {
        public override string Code => "Habari.Action.File.Read";

        [Output("content", "File content", typeof(byte[]), typeof(string))]
        public Output Content => Outputs["content"];

        public override string Description => "Read the content of a file";

        [Output("fileNotFound", "File not found", typeof(bool))]
        public Output FileNotFound => Outputs["fileNotFound"];

        public override string Name => "";

        [Input("path", "Asked path", true, typeof(byte[]), typeof(string))]
        public Input Path => Inputs["path"];

        [Constant("rootDirectory", "Root directory", true)]
        public string RootDirectory { get; set; } = "./www";

        public Read()
        {
        }

        /*
        public override void LoadConstants(JsonObject config)
        {
            if(config["rootDirectory"] != null)
                RootDirectory = config["rootDirectory"]!.GetValue<string>() ?? "./www";
        }
        */

        public override async Task RunAsync(WorkflowContext context)
        {
            string path = Path.GetValue<string>(context)!;
            if (path.StartsWith('/'))
                path = path.Substring(1);

            try
            {
                byte[] fileContentBytes = await IO.File.ReadAllBytesAsync(IO.Path.Combine(RootDirectory, path));
                string fileContentString = await IO.File.ReadAllTextAsync(IO.Path.Combine(RootDirectory, path));
                Content.SetValue(context, (typeof(byte[]), fileContentBytes), (typeof(string), fileContentString));
                FileNotFound.SetValue(context, (typeof(bool), false));
            }
            catch
            {
                string fileContentString = string.Empty;
                byte[] fileContentBytes = Encoding.Default.GetBytes(fileContentString);
                Content.SetValue(context, (typeof(byte[]), fileContentBytes), (typeof(string), fileContentString));
                FileNotFound.SetValue(context, (typeof(bool), true));
            }
        }
    }
}

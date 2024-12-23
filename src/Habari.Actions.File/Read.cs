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

        public OutputParameter Content => Outputs["content"];

        public override string Description => "Read the content of a file";

        public OutputParameter FileNotFound => Outputs["fileNotFound"];

        public override string Name => "";

        public InputParameter Path => Inputs["path"];

        public string RootDirectory { get; set; } = "./www";

        public Read()
        {
            Inputs.Add(new InputParameter(this, "path", "Asked path", true, typeof(byte[]), typeof(string)));
            Outputs.Add(new OutputParameter(this, "content", "File content", typeof(byte[]), typeof(string)));
            Outputs.Add(new OutputParameter(this, "fileNotFound", "File not found", typeof(bool)));
        }

        public override void LoadCustomParameters(JsonObject config)
        {
            try
            {
                RootDirectory = config["rootDirectory"]!.GetValue<string>() ?? "./www";
            }
            catch
            {
                RootDirectory = "./www";
            }
        }

        public override void Run(WorkflowContext context)
        {
            string path = Path.GetValue<string>(context)!;
            if (path.StartsWith('/'))
            {
                path = path.Substring(1);
            }
            try
            {
                byte[] fileContentBytes = IO.File.ReadAllBytes(IO.Path.Combine(RootDirectory, path));
                string fileContentString = IO.File.ReadAllText(IO.Path.Combine(RootDirectory, path));
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

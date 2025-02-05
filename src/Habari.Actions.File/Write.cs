using Habari.Library.Parameters;
using Habari.Library.Steps;
using IO = System.IO;

namespace Habari.Actions.File;

public class Write : Step
{
    public override string Code => "Habari.Action.File.Write";

    [Input("content", "File content", ParameterType.Text, typeof(byte[]), typeof(string))]
    public Input Content => Inputs["content"];

    public override string Description => "Write the content into a file";

    [Output("fileCreated", "File created", ParameterType.Boolean, typeof(bool))]
    public Output FileCreated => Outputs["fileCreated"];

    public override string Name => "File writer";

    [Input("path", "Asked path", ParameterType.Path, true, typeof(byte[]), typeof(string))]
    public Input Path => Inputs["path"];

    [Constant("rootDirectory", "Root directory", ParameterType.Path, true)]
    public string RootDirectory { get; set; } = "./upload";

    public override async Task RunAsync(WorkflowContext context)
    {
        string path = Path.GetValue<string>(context)!;
        byte[] content = Content.GetValue<byte[]>(context)!;
        if (path.StartsWith('/'))
            path = path.Substring(1);

        try
        {
            await IO.File.WriteAllBytesAsync(IO.Path.Combine(RootDirectory, path), content);
            FileCreated.SetValue(context, (typeof(bool), true));
        }
        catch
        {
            FileCreated.SetValue(context, (typeof(bool), false));
        }
    }
}

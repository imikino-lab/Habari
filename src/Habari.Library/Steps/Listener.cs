using Habari.Library.Json;
using Habari.Library.Parameters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Habari.Library.Steps;

public abstract class Listener : IListener
{
    public abstract string Code { get; }

    [JsonConverter(typeof(ConstantsJsonConverter))]
    public Constants Constants { get; } = new();

    public abstract string Description { get; }

    public int Height { get; set; }

    public abstract string Name { get; }

    public List<ITrigger> Triggers { get; protected set; } = new();

    public int Width { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public abstract void Load(JsonObject config);

    public abstract void LoadTrigger(JsonObject config);

    public abstract void Start(WorkflowContext context);

    public abstract void Stop();

    public Listener()
    {
        CreateConstants();
    }

    private void CreateConstants()
    {
        var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(ConstantAttribute), false).Any());
        foreach (var property in properties)
        {
            var attribute = (ConstantAttribute)property.GetCustomAttributes(typeof(ConstantAttribute), false).First();
            Constants.Add(new Constant(this, attribute.Code, attribute.Name, attribute.ParameterType, attribute.IsRequired));
        }
    }
}

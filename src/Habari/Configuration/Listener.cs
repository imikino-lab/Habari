using Habari.Library;
using System.Text.Json.Serialization;

namespace Habari.Configuration;

internal class Listener
{
    public Listener(Type type)
    {
        if (type == null)
        {
            return;
        }

        var element = Activator.CreateInstance(type) as Library.Steps.Listener;
        if (element != null)
        {
            Code = element.Code;
            Description = element.Description;
            Name = element.Name;
            PosX = element.X;
            PosY = element.Y;
            Type = element.GetType().Name;
            foreach (var trigger in ConfigurationManager.Instance.AvailableTriggers[element.Code].Values)
            {
                AvailableTriggers.Add(new Trigger(trigger));
            }
        }
    }

    [JsonPropertyName("class")]
    public string Code { get; set; } = "";

    [JsonIgnore]
    public string Description { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonIgnore]
    public List<Trigger> AvailableTriggers { get; set; } = new();

    [JsonPropertyName("pos_x")]
    public float PosX { get; set; } = 0;

    [JsonPropertyName("pos_y")]
    public float PosY { get; set; } = 0;

    [JsonIgnore]
    public string Type { get; set; } = "";
}

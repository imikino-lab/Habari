using System.Text.Json.Serialization;

namespace Habari.Configuration;

internal class Step
{
    public Step(Type type)
    {
        if (type == null)
        {
            return;
        }

        var element = Activator.CreateInstance(type) as Library.Steps.Step;
        if (element != null)
        {
            Init(element);
        }
    }

    public Step(Library.Steps.Step step)
    {
        Init(step);
    }

    private void Init(Library.Steps.Step step)
    {
        Code = step.Code;
        Description = step.Description;
        Id = step.Id;
        Name = step.Name;
        PosX = step.X;
        PosY = step.Y;
        Type = step.GetType().Name;
    }

    [JsonPropertyName("class")]
    public string Code { get; set; } = "";

    [JsonIgnore]
    public string Description { get; set; } = "";

    [JsonPropertyName("html")]
    public string Html { get; set; } = "";

    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("pos_x")]
    public float PosX { get; set; } = 0;

    [JsonPropertyName("pos_y")]
    public float PosY { get; set; } = 0;

    [JsonIgnore]
    public string Type { get; set; } = "";
}

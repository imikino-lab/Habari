namespace Habari.Configuration;

internal class Trigger
{
    public Trigger(Type type)
    {
        if (type == null)
        {
            return;
        }

        var element = Activator.CreateInstance(type) as Library.Steps.Trigger;
        if (element != null)
        {
            Code = element.Code;
            Description = element.Description;
            Name = element.Name;
            Type = element.GetType().Name;
        }
    }

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public string Name { get; set; } = "";

    public string Type { get; set; } = "";
}

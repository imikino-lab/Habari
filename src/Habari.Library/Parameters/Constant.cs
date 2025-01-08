using Habari.Library.Steps;
using System.Text.Json.Serialization;

namespace Habari.Library.Parameters;

public class Constant
{
    public string Code { get; private set; }

    public bool IsRequired { get; private set; }

    public string Name { get; private set; }

    [JsonIgnore]
    public IBase Step { get; private set; }

    public Constant(IBase step, string code, string name, bool isRequired)
    {
        Code = code.ToLower();
        IsRequired = isRequired;
        Name = name;
        Step = step;
    }
}

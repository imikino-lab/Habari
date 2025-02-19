using Habari.Library.Parameters;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps;

public interface IRoot
{
    string Code { get; }

    Constants Constants { get; }

    string Description { get; }

    int Height { get; set; }

    string Name { get; }

    int Width { get; set; }

    int X { get; set; }

    int Y { get; set; }

    void Load(JsonObject config);
}

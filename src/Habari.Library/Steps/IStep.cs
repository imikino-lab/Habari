using Habari.Library.Json;

namespace Habari.Library.Steps;

[InterfaceJsonConverter(typeof(InterfaceJsonConverter<IStep>))]
public interface IStep : IBase
{
}

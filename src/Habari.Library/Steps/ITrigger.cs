using Habari.Library.Json;

namespace Habari.Library.Steps
{
    [InterfaceJsonConverter(typeof(InterfaceJsonConverter<ITrigger>))]
    public interface ITrigger : IBase
    {
    }
}

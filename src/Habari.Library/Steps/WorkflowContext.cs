using System.ComponentModel;
using System.Linq;

namespace Habari.Library.Steps
{
    public class WorkflowContext
    {
        private Dictionary<string, Dictionary<Type, object?>> Data { get; } = new ();

        public void Set(string key, params (Type, object?)[] values)
        {
            Data[key] = values.Select(value => new KeyValuePair<Type, object?>(value.Item1, value.Item2)).ToDictionary();
        }

        public void Remove(string key)
        {
            Data.Remove(key);
        }

        public object? Get(string key, Type type)
        {
            return Data![key]![type];
        }
    }
}

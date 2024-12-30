using Habari.Library.Steps;

namespace Habari.Library.Parameters
{
    public class Constants
    {
        public static Constants From(IStep step)
        {
            Constants constants = new();
            /*
            foreach (IConfigurationParameter input in step.GetType().Attributes.Where(attribute => attribute.))
            {
                configurationParameters.Add(new ConfigurationParameter(step, input.Code, input.Name, input.IsRequired, input.Types));
            }
            */
            return constants;
        }


        private Dictionary<string, Constant> _innerDictionary = new();

        public Constant this[string key]
        {
            get
            {
                return _innerDictionary[key.ToLower()];
            }
            set
            {
                if (_innerDictionary.ContainsKey(key.ToLower()))
                {
                    _innerDictionary.Add(key.ToLower(), value);
                }
                else
                {
                    _innerDictionary[key.ToLower()] = value;
                }
            }
        }

        public ICollection<string> Keys => _innerDictionary.Keys;

        public ICollection<Constant> Values => _innerDictionary.Values;

        public int Count => _innerDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(Constant value)
        {
            _innerDictionary.Add(value.Code.ToLower(), value);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key.ToLower());
        }

        public IEnumerator<Constant> GetEnumerator()
        {
            return _innerDictionary.Values.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out Constant? value)
        {
            return _innerDictionary.TryGetValue(key.ToLower(), out value);
        }
    }
}
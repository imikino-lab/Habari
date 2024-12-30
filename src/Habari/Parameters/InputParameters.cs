using Habari.Library.Parameters;

namespace Habari.Parameters
{
    public class InputParameters
    {

        private Dictionary<string, Input> _innerDictionary = new();

        public Input this[string key]
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

        public ICollection<Input> Values => _innerDictionary.Values;

        public int Count => _innerDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(Input value)
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

        public IEnumerator<Input> GetEnumerator()
        {
            return _innerDictionary.Values.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out Input? value)
        {
            return _innerDictionary.TryGetValue(key.ToLower(), out value);
        }

        public bool ValidateLink()
        {
            return Values.All(value => value.IsLinked && value.IsRequired) || Values.All(value => !value.IsLinked && value.IsRequired);
        }
    }
}

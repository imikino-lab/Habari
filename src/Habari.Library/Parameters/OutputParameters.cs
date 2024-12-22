namespace Habari.Library.Parameters
{
    public class OutputParameters
    {

        private Dictionary<string, OutputParameter> _innerDictionary = new Dictionary<string, OutputParameter>();

        public OutputParameter this[string key]
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

        public ICollection<OutputParameter> Values => _innerDictionary.Values;

        public int Count => _innerDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(OutputParameter value)
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

        public IEnumerator<OutputParameter> GetEnumerator()
        {
            return _innerDictionary.Values.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out OutputParameter? value)
        {
            return _innerDictionary.TryGetValue(key.ToLower(), out value);
        }
    }
}

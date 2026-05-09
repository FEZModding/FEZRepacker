using System.Collections;

using FEZRepacker.Core.Definitions.Game;

namespace FEZRepacker.Core.Helpers
{
    [XnbType("System.Collections.Generic.Dictionary, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.DictionaryReader")]
    internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly List<TKey> _keys;
        private readonly List<TValue> _values;
        
        public int Count => _dictionary.Count;
        public ICollection<TKey> Keys => _keys.AsReadOnly();
        public ICollection<TValue> Values => _values.AsReadOnly();
        
        public TValue this[TKey key] 
        {
            get
            {
                return _dictionary[key];
            }
            set 
            {
                RemoveFromLists(key);

                _dictionary[key] = value;
                _keys.Add(key);
                _values.Add(value);
            }
        }
        
        public OrderedDictionary() : this(0) { }

        public OrderedDictionary(int capacity) 
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
            _keys = new List<TKey>(capacity);
            _values = new List<TValue>(capacity);
        }

        public void Add(TKey key, TValue value) 
        {
            _dictionary.Add(key, value);
            _keys.Add(key);
            _values.Add(value);
        }

        public void Clear() 
        {
            _dictionary.Clear();
            _keys.Clear();
            _values.Clear();
        }

        public bool ContainsKey(TKey key) 
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value) 
        {
            return _dictionary.ContainsValue(value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
        {
            for (var i = 0; i < _keys.Count; i++) 
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        private void RemoveFromLists(TKey key) 
        {
            int index = _keys.IndexOf(key);
            if (index == -1)
            {
                return;
            }

            _keys.RemoveAt(index);
            _values.RemoveAt(index);
        }

        public bool Remove(TKey key) 
        {
            RemoveFromLists(key);
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) 
        {
            return _dictionary.TryGetValue(key, out value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => 
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;
        
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
            Add(item.Key, item.Value);
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) 
        {
            bool removed = ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);

            if (removed) {
                RemoveFromLists(item.Key);
            }

            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
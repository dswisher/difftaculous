﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Difftaculous.ZModel
{
    internal class ZPropertyKeyedCollection : Collection<ZToken>
    {

        private static readonly IEqualityComparer<string> Comparer = StringComparer.Ordinal;

        private Dictionary<string, ZToken> _dictionary;


        private void AddKey(string key, ZToken item)
        {
            EnsureDictionary();
            _dictionary[key] = item;
        }



#if false
        protected void ChangeItemKey(JToken item, string newKey)
        {
            if (!ContainsItem(item))
                throw new ArgumentException("The specified item does not exist in this KeyedCollection.");

            string keyForItem = GetKeyForItem(item);
            if (!Comparer.Equals(keyForItem, newKey))
            {
                if (newKey != null)
                    AddKey(newKey, item);

                if (keyForItem != null)
                    RemoveKey(keyForItem);
            }
        }
#endif

        protected override void ClearItems()
        {
            base.ClearItems();

            if (_dictionary != null)
                _dictionary.Clear();
        }



        public bool Contains(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_dictionary != null)
                return _dictionary.ContainsKey(key);

            return false;
        }


#if false
        private bool ContainsItem(JToken item)
        {
            if (_dictionary == null)
                return false;

            string key = GetKeyForItem(item);
            JToken value;
            return _dictionary.TryGetValue(key, out value);
        }
#endif


        private void EnsureDictionary()
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<string, ZToken>(Comparer);
            }
        }


        private string GetKeyForItem(ZToken item)
        {
            return ((ZProperty)item).Name;
        }


        protected override void InsertItem(int index, ZToken item)
        {
            AddKey(GetKeyForItem(item), item);
            base.InsertItem(index, item);
        }



#if false
        public bool Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_dictionary != null)
                return _dictionary.ContainsKey(key) && Remove(_dictionary[key]);

            return false;
        }

        protected override void RemoveItem(int index)
        {
            string keyForItem = GetKeyForItem(Items[index]);
            RemoveKey(keyForItem);
            base.RemoveItem(index);
        }

        private void RemoveKey(string key)
        {
            if (_dictionary != null)
                _dictionary.Remove(key);
        }

        protected override void SetItem(int index, JToken item)
        {
            string keyForItem = GetKeyForItem(item);
            string keyAtIndex = GetKeyForItem(Items[index]);

            if (Comparer.Equals(keyAtIndex, keyForItem))
            {
                if (_dictionary != null)
                    _dictionary[keyForItem] = item;
            }
            else
            {
                AddKey(keyForItem, item);

                if (keyAtIndex != null)
                    RemoveKey(keyAtIndex);
            }
            base.SetItem(index, item);
        }

        public JToken this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (_dictionary != null)
                    return _dictionary[key];

                throw new KeyNotFoundException();
            }
        }
#endif


        public bool TryGetValue(string key, out ZToken value)
        {
            if (_dictionary == null)
            {
                value = null;
                return false;
            }

            return _dictionary.TryGetValue(key, out value);
        }



        public ICollection<string> Keys
        {
            get
            {
                EnsureDictionary();
                return _dictionary.Keys;
            }
        }

        public ICollection<ZToken> Values
        {
            get
            {
                EnsureDictionary();
                return _dictionary.Values;
            }
        }


#if false
        public bool Compare(JPropertyKeyedCollection other)
        {
            if (this == other)
                return true;

            // dictionaries in JavaScript aren't ordered
            // ignore order when comparing properties
            Dictionary<string, JToken> d1 = _dictionary;
            Dictionary<string, JToken> d2 = other._dictionary;

            if (d1 == null && d2 == null)
                return true;

            if (d1 == null)
                return (d2.Count == 0);

            if (d2 == null)
                return (d1.Count == 0);

            if (d1.Count != d2.Count)
                return false;

            foreach (KeyValuePair<string, JToken> keyAndProperty in d1)
            {
                JToken secondValue;
                if (!d2.TryGetValue(keyAndProperty.Key, out secondValue))
                    return false;

                JProperty p1 = (JProperty)keyAndProperty.Value;
                JProperty p2 = (JProperty)secondValue;

                if (p1.Value == null)
                    return (p2.Value == null);

                if (!p1.Value.DeepEquals(p2.Value))
                    return false;
            }

            return true;
        }
#endif

    }
}
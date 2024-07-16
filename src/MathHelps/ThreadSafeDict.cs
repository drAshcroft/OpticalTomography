using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathHelpLib
{
    public class DictionaryThreadSafe<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private object CriticalSectionLock = new object();

        public new TValue this[TKey key]
        {
            get
            {
                lock (CriticalSectionLock)
                {
                    return base[key];
                }
            }
            set
            {
                lock (CriticalSectionLock)
                {
                     base[key]=value ;
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            lock (CriticalSectionLock)
            {
                base.Add(key, value);
            }
        }

        public void AddSafe(TKey key, TValue value)
        {
            lock (CriticalSectionLock)
            {
                if (base.ContainsKey(key))
                    base.Remove(key);
                base.Add(key, value);
            }
        }

        public new bool ContainsKey(TKey key)
        {
            lock (CriticalSectionLock)
            {
                return base.ContainsKey(key);
            }
        }

        public new bool Remove(TKey key)
        {
            lock (CriticalSectionLock)
            {
                return base.Remove(key);
            }
        }
    }
}

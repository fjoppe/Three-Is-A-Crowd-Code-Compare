using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace LE.Application.Classes
{
    public class TwoWayMapper<TK1, TK2>: IEnumerable<TK1>
    {
        private Dictionary<TK1, TK2> mapping1;
        private Dictionary<TK2, TK1> mapping2;

        public TwoWayMapper()
        {
            mapping1 = new Dictionary<TK1, TK2>();
            mapping2 = new Dictionary<TK2, TK1>();
        }


        public void Add(TK1 key1, TK2 key2)
        {
            mapping1.Add(key1, key2);
            mapping2.Add(key2, key1);
        }

        public void Remove(TK1 key)
        {
            mapping2.Remove(mapping1[key]);
            mapping1.Remove(key);
        }

        public void Remove(TK2 key)
        {
            mapping1.Remove(mapping2[key]);
            mapping2.Remove(key);
        }

        public void Clear()
        {
            mapping1.Clear();
            mapping2.Clear();
        }


        public TK2 this[TK1 key]
        {
            get
            {
                return this.mapping1[key];
            }
        }


        public TK1 this[TK2 key]
        {
            get
            {
                return this.mapping2[key];
            }
        }


        public IEnumerator<TK1> GetEnumerator()
        {
            return mapping1.Keys.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return mapping1.Keys.GetEnumerator();
        }


        /// <summary>
        /// Returns enumerator of all TK2 keys.
        /// </summary>
        /// <returns></returns>
        public TK2[] GetArrayK2()
        {
            return mapping2.Keys.ToArray();
        }

    }
}

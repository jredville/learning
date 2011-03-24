using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinLINQ
{
    class Counter : IRecord<Func<int>, Action<int>>
    {
        private int _value;

        Func<int> IRecord<Func<int>, Action<int>>.First { get { return () => _value; } }
        Action<int> IRecord<Func<int>, Action<int>>.Second { get { return i => _value += i; } }

        public Func<int> Get { get { return ((IRecord<Func<int>, Action<int>>)this).First; } }
        public Action<int> Inc { get { return ((IRecord<Func<int>, Action<int>>)this).Second; } }
    }
}

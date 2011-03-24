using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinLINQ
{
    public abstract class Option<T>
    {
        public abstract bool HasValue { get; }
        public abstract T Value { get; }
        public sealed class None : Option<T>
        {
            public override bool HasValue
            {
                get { return false; }
            }

            public override T Value
            {
                get { throw new InvalidOperationException(); }
            }

            public override string ToString()
            {
                return "None<" + typeof(T).Name + ">()";
            }
        }

        public sealed class Some : Option<T> {
            private T _value;

            public Some(T value)
            {
                _value = value;
            }

            public override bool HasValue
            {
                get { return true; }
            }

            public override T Value
            {
                get { return _value; }
            }

            public override string ToString()
            {
                return "Some<" + typeof(T).Name + ">(" + (_value == null ? "null" : _value.ToString()) + ")";
            }
        }
    }
}

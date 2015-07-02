using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public static class HarshSingleElementCollection
    {
        public static IEnumerable<T> Create<T>(T value) => new HarshSingleElementCollection<T>(value);
    }

    public sealed class HarshSingleElementCollection<T> : IEnumerable<T>
    {
        private readonly T _value;

        public HarshSingleElementCollection(T value)
        {
            _value = value;
        }

        public Enumerator GetEnumerator() => new Enumerator(_value);
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T _value;
            private Boolean _enumerated;

            public Enumerator(T value)
            {
                _value = value;
                _enumerated = false;
            }

            public T Current => _value;

            Object IEnumerator.Current => Current;

            public void Dispose() => _enumerated = true;

            public Boolean MoveNext() => _enumerated ? false : _enumerated = true;

            public void Reset() => _enumerated = false;
        }
    }
}

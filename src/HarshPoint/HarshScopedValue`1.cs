using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HarshPoint
{
    public sealed class HarshScopedValue<T>
    {
        public HarshScopedValue() { }

        public HarshScopedValue(T value)
        {
            Value = value;
        }

        public Scope Enter(T value)
        {
            var scope = new Scope(this, Value);
            Value = value;
            return scope;
        }

        public Scope EnterIfDefault(T value)
            => EnterIfDefault(value, null);

        public Scope EnterIfDefault(T value, IEqualityComparer<T> equalityComparer)
        {
            if (equalityComparer == null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }

            if (equalityComparer.Equals(Value, default(T)))
            {
                return Enter(value);
            }

            return Scope.Empty;
        }

        public T Value
        {
            get;
            private set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public struct Scope : IDisposable
        {
            private T OldValue;
            private HarshScopedValue<T> Owner;

            internal Scope(HarshScopedValue<T> owner, T oldValue)
            {
                Owner = owner;
                OldValue = oldValue;
            }

            public void Dispose()
            {
                if (Owner != null)
                {
                    Owner.Value = OldValue;

                    Owner = null;
                    OldValue = default(T);
                }
            }

            internal static readonly Scope Empty;
        }
    }
}

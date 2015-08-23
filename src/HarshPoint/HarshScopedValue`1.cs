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
            HasValue = true;
        }

        public Scope Enter(T value)
        {
            var scope = new Scope(this, Value, HasValue);
            Value = value;
            HasValue = true;
            return scope;
        }

        public Scope EnterIfHasNoValue(T value)
        {
            if (!HasValue)
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

        public Boolean HasValue { get; private set; }

        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public struct Scope : IDisposable
        {
            private T OldValue;
            private Boolean OldHasValue;
            private HarshScopedValue<T> Owner;

            internal Scope(HarshScopedValue<T> owner, T oldValue, Boolean oldHasValue)
            {
                Owner = owner;
                OldValue = oldValue;
                OldHasValue = oldHasValue;
            }

            public void Dispose()
            {
                if (Owner != null)
                {
                    Owner.Value = OldValue;
                    Owner.HasValue = OldHasValue;

                    Owner = null;
                    OldValue = default(T);
                }
            }

            internal static readonly Scope Empty;
        }
    }
}

using System;

namespace HarshPoint.Provisioning
{
    public abstract class DefaultFromContextTag<T> : IDefaultFromContextTag
    {
        public T Value
        {
            get;
            set;
        }

        Object IDefaultFromContextTag.Value => Value;
    }
}

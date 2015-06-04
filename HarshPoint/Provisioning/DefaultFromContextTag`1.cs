using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Globalization;

namespace HarshPoint
{
    internal sealed class HarshTraceSource
    {
        public HarshTraceSource(Type owner)
        {
            if (owner == null)
            {
                throw Error.ArgumentNull("owner");
            }

            Owner = owner;
        }

        public Type Owner
        {
            get;
            private set;
        }

        public void WriteLine(String message)
        {
            HarshTrace.WriteLine("{0}: {1}", Owner.Name, message);
        }

        public void WriteLine(String format, params Object[] args)
        {
            HarshTrace.WriteLine(
                "{0}: {1}", 
                Owner.Name, 
                String.Format(CultureInfo.InvariantCulture, format, args)
            );
        }
    }
}

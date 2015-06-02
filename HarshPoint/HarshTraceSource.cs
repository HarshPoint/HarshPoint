using System;
using System.Globalization;
using System.Reflection;

namespace HarshPoint
{
    [Obsolete]
    internal sealed class HarshTraceSource
    {
        public HarshTraceSource(Type owner)
        {
            if (owner == null)
            {
                throw Error.ArgumentNull(nameof(owner));
            }

            Name = owner.GetTypeInfo().GetCSharpSimpleName();
        }

        public HarshTraceSource(String name)
            : this(name, null)
        {
        }

        public HarshTraceSource(String name, HarshTraceSource parent)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhitespace(nameof(name));
            }

            Name = name;
            Parent = parent;
        }

        public String Name
        {
            get;
            private set;
        }

        public HarshTraceSource Parent
        {
            get;
            private set;
        }

        public override String ToString()
        {
            if (Parent != null)
            {
                return Parent.ToString() + ": " + Name;
            }

            return Name;
        }

        public void WriteError(Exception exception)
        {
            if (exception == null)
            {
                throw Error.ArgumentNull(nameof(exception));
            }

            HarshTrace.WriteError(
                PrefixThis(exception.ToString()),
                exception
            );
        }

        public void WriteInfo(String message)
        {
            if (message == null)
            {
                throw Error.ArgumentNull(nameof(message));
            }

            HarshTrace.WriteInfo(
                PrefixThis(message)
            );
        }

        public void WriteInfo(String format, params Object[] args)
        {
            if (format == null)
            {
                throw Error.ArgumentNull(nameof(format));
            }

            if (args == null)
            {
                throw Error.ArgumentNull(nameof(args));
            }

            HarshTrace.WriteInfo(
                PrefixThis(
                    FormatString(format, args)
                )
            );
        }

        private String PrefixThis(String message)
        {
            return FormatString("{0}: {1}", this, message);
        }

        private static String FormatString(String format, params Object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}

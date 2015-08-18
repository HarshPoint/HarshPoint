// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  FormattableString
**
**
** Purpose: implementation of the FormattableString
** class.
**
===========================================================*/
namespace System
{
    /// <summary>
    /// A composite format string along with the arguments to be formatted. An instance of this
    /// type may result from the use of the C# or VB language primitive "interpolated string".
    /// </summary>
    public abstract class FormattableString : IFormattable
    {
        /// <summary>
        /// The composite format string.
        /// </summary>
        public abstract String Format { get; }
        /// <summary>
        /// Returns an object array that contains zero or more objects to format. Clients should not
        /// mutate the contents of the array.
        /// </summary>
        public abstract Object[] GetArguments();
        /// <summary>
        /// The number of arguments to be formatted.
        /// </summary>
        public abstract Int32 ArgumentCount { get; }
        /// <summary>
        /// Returns one argument to be formatted from argument position <paramref name="index"/>.
        /// </summary>
        public abstract Object GetArgument(Int32 index);
        /// <summary>
        /// Format to a string using the given culture.
        /// </summary>
        public abstract String ToString(IFormatProvider formatProvider);
        String IFormattable.ToString(String ignored, IFormatProvider formatProvider) => ToString(formatProvider);
        /// <summary>
        /// Format the given object in the invariant culture. This static method may be
        /// imported in C# by
        /// <code>
        /// using static System.FormattableString;
        /// </code>.
        /// Within the scope
        /// of that import directive an interpolated string may be formatted in the
        /// invariant culture by writing, for example,
        /// <code>
        /// Invariant($"{{ lat = {latitude}; lon = {longitude} }}")
        /// </code>
        /// </summary>
        public static String Invariant(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(Globalization.CultureInfo.InvariantCulture);
        }

        public override String ToString()
            => ToString(Globalization.CultureInfo.CurrentCulture);
    }
}
// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  FormattableStringFactory
**
**
** Purpose: implementation of the FormattableStringFactory
** class.
**
===========================================================*/
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// A factory type used by compilers to create instances of the type <see cref="FormattableString"/>.
    /// </summary>
    public static class FormattableStringFactory
    {
        /// <summary>
        /// Create a <see cref="FormattableString"/> from a composite format string and object
        /// array containing zero or more objects to format.
        /// </summary>
        public static FormattableString Create(String format, params Object[] arguments)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return new ConcreteFormattableString(format, arguments);
        }
        private sealed class ConcreteFormattableString : FormattableString
        {
            private readonly String _format;
            private readonly Object[] _arguments;
            internal ConcreteFormattableString(String format, Object[] arguments)
            {
                _format = format;
                _arguments = arguments;
            }
            public override String Format => _format;
            public override Object[] GetArguments() => _arguments;
            public override Int32 ArgumentCount => _arguments.Length;
            public override Object GetArgument(Int32 index) => _arguments[index];
            public override String ToString(IFormatProvider formatProvider) => String.Format(formatProvider, Format, _arguments);
        }
    }
}
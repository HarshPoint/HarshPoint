using System;
using System.Text.RegularExpressions;

namespace HarshPoint
{
    public sealed class HarshContentTypeId : IEquatable<HarshContentTypeId>
    {
        private static readonly Regex CTIdRegex = new Regex(@"(?x)
            ^
            (?<IsAbsolute> 0x)?
            (?<Value>      [A-Fa-f0-9]*)
            $
        ");

        private readonly String _value;

        private HarshContentTypeId(String value)
        {
            _value = value.ToUpperInvariant();
        }

        public override Boolean Equals(Object obj) => Equals(obj as HarshContentTypeId);

        public Boolean Equals(HarshContentTypeId other)
        {
            if (other == null)
            {
                return false;
            }

            return 
                IsAbsolute.Equals(other.IsAbsolute) &&
                _value.Equals(other._value);
        }

        public override Int32 GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override String ToString()
        {
            if (IsAbsolute)
            {
                return "0x" + _value;
            }

            return _value;
        }

        public Boolean IsAbsolute
        {
            get;
            private set;
        }

        public Boolean IsChildOf(HarshContentTypeId parent)
        {
            if (parent == null)
            {
                throw Error.ArgumentNull(nameof(parent));
            }

            if (!parent.IsAbsolute)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    nameof(parent),
                    SR.HarshContentTypeId_CannotIsChildOfRelative,
                    parent
                );
            }

            if (!IsAbsolute)
            {
                throw Error.InvalidOperation(
                    SR.HarshContentTypeId_CannotIsChildOfRelative,
                    this
                );
            }

            if (_value.Length <= parent._value.Length)
            {
                return false;
            }

            return _value.StartsWith(parent._value);
        }

        public Boolean IsDirectChildOf(HarshContentTypeId parent)
        {
            if (!IsChildOf(parent))
            {
                return false;
            }

            var otherLength = parent._value.Length;

            if (_value.Length == otherLength + 2 || 
                _value.Length == otherLength + 34)
            {
                return true;
            }

            return false;
        }

        public HarshContentTypeId Append(HarshContentTypeId contentTypeId)
        {
            if (contentTypeId == null)
            {
                throw Error.ArgumentNull("contentTypeId");
            }

            if (contentTypeId.IsAbsolute)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_CannotAppendAbsoluteCTId,
                    contentTypeId,
                    this
                );
            }

            var result = _value;

            if (contentTypeId._value.Length > 2)
            {
                result += "00";
            }

            result += contentTypeId._value;

            return new HarshContentTypeId(result)
            {
                IsAbsolute = this.IsAbsolute
            };
        }

        public static HarshContentTypeId Parse(String contentTypeId)
        {
            if (contentTypeId == null)
            {
                throw Error.ArgumentNull("contentTypeId");
            }

            if (!IsEven(contentTypeId.Length))
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_NotEven,
                    contentTypeId
                );
            }

            var match = CTIdRegex.Match(contentTypeId);

            if (!match.Success)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_IllegalCharacters,
                    contentTypeId
                );
            }

            if (match.Groups["IsAbsolute"].Success)
            {
                return ParseAbsolute(match.Groups["Value"].Value);
            }

            return ParseRelative(contentTypeId);
        }

        private static HarshContentTypeId ParseAbsolute(String contentTypeId)
        {
            for (var i = 0; i < contentTypeId.Length; i += 2)
            {
                if (StringComparer.Ordinal.Equals("00", contentTypeId.Substring(i, 2)))
                {
                    i += 32;
                }

                if (i > contentTypeId.Length)
                {
                    throw Error.ArgumentOutOfRangeFormat(
                        "contentTypeId",
                        SR.HarshContentTypeId_Expected_32chars_ID_after_00,
                        contentTypeId
                    );
                }
            }

            return new HarshContentTypeId(contentTypeId)
            {
                IsAbsolute = true
            };
        }

        private static HarshContentTypeId ParseRelative(String contentTypeId)
        {
            if (contentTypeId.Length != 2 && contentTypeId.Length != 32)
            {
                throw Error.ArgumentOutOfRangeFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_RelCTId_OutOfRange,
                    contentTypeId
                );
            }

            if (StringComparer.Ordinal.Equals(contentTypeId, "00"))
            {
                throw Error.ArgumentOutOfRange("contentTypeId", SR.HarshContentTypeId_RelCTId_00_OutOfRange);
            }

            return new HarshContentTypeId(contentTypeId);
        }

        private static Boolean IsEven(Int32 value)
        {
            return (value % 2) == 0;
        }
    }
}

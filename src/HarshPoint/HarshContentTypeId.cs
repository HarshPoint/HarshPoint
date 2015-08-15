using Microsoft.SharePoint.Client;
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
            => _value.GetHashCode();

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
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            if (!parent.IsAbsolute)
            {
                throw Logger.Fatal.ArgumentFormat(
                    nameof(parent),
                    SR.HarshContentTypeId_CannotIsChildOfRelative,
                    parent
                );
            }

            if (!IsAbsolute)
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.HarshContentTypeId_CannotIsChildOfRelative,
                    this
                );
            }

            if (_value.Length <= parent._value.Length)
            {
                return false;
            }

            return _value.StartsWith(parent._value, StringComparison.Ordinal);
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
                throw Logger.Fatal.ArgumentNull("contentTypeId");
            }

            if (contentTypeId.IsAbsolute)
            {
                throw Logger.Fatal.ArgumentFormat(
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
                throw Logger.Fatal.ArgumentNull("contentTypeId");
            }

            if (!IsEven(contentTypeId.Length))
            {
                throw Logger.Fatal.ArgumentFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_NotEven,
                    contentTypeId
                );
            }

            var match = CTIdRegex.Match(contentTypeId);

            if (!match.Success)
            {
                throw Logger.Fatal.ArgumentFormat(
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

        public static HarshContentTypeId Get(ContentType contentType)
        {
            if (contentType == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(contentType));
            }

            return Parse(contentType.StringId);
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
                    throw Logger.Fatal.ArgumentFormat(
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
                throw Logger.Fatal.ArgumentFormat(
                    "contentTypeId",
                    SR.HarshContentTypeId_RelCTId_Invalid,
                    contentTypeId
                );
            }

            if (StringComparer.Ordinal.Equals(contentTypeId, "00"))
            {
                throw Logger.Fatal.Argument("contentTypeId", SR.HarshContentTypeId_RelCTId_00_Invalid);
            }

            return new HarshContentTypeId(contentTypeId);
        }

        private static Boolean IsEven(Int32 value) => (value % 2) == 0;

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshContentTypeId>();
    }
}

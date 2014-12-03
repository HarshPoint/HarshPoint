using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HarshPoint
{
    public sealed class HarshContentTypeId
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

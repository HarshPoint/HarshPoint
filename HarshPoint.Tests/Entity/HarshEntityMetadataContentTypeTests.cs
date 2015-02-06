using HarshPoint.Entity.Metadata;
using System;
using Xunit;

namespace HarshPoint.Tests.Entity
{
    public class HarshEntityMetadataContentTypeTests
    {
        [Fact]
        public void Fails_without_a_ContentTypeAttribute()
        {
            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => new HarshEntityMetadataContentType(typeof(NotAContentType))
            );
        }

        private sealed class NotAContentType
        {
        }
    }
}

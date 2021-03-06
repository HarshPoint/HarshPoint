﻿using HarshPoint.Provisioning;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class ContentTypeResolving : ResolverTestBase
    {
        public ContentTypeResolving(ITestOutputHelper output)
            : base(output)
        {
        }

        [TheoryNeedsSharePoint]
        [InlineData("0x01")]
        [InlineData("0x0101")]
        [InlineData("0x0120")]
        public async Task Valid_id_gets_resolved(String id)
        {
            var resolver = Resolve.ContentType().ById(HarshContentTypeId.Parse(id));
            var ct = Assert.Single(await ResolveAsync(resolver));

            Assert.False(ct.IsNull());
            Assert.Equal(id, await ct.EnsurePropertyAvailable(c => c.StringId));
        }
    }
}

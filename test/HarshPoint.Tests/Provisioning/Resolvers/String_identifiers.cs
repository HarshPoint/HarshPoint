using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using System.Collections;
using System.Collections.Immutable;
using Xunit;

namespace HarshPoint.Tests.Provisioning.Resolvers
{
    public class String_identifiers : SharePointClientTest
    {
        public String_identifiers(ITestOutputHelper output) : base(output)
        {
        }

        // https://github.com/Turo-NET/UnicodeNormalization/ isn't exactly sane

        [Fact(Skip = "PCL doesnt support Normalize. Need to find a sane implementation.")]
        public void With_composite_chars_resolve()
        {
            var idc = "žščřöůýÿíé".Normalize(NormalizationForm.FormC);
            var idd = "žščřöůýÿíé".Normalize(NormalizationForm.FormD);

            var parent = new ParentRB(idc);
            var id = new IdResolveBuilder(parent, new[] { idd });

            var result = new ManualResolver(CreateResolveContext).Resolve(id);
            var str = Assert.Single(result);

            Assert.Same(idc, str);
        }

        private class ParentRB :
            ResolveBuilder<String, ResolveContext<HarshProvisionerContext>>
        {
            private readonly String _str;

            public ParentRB(String str)
            {
                _str = str;
            }

            protected override IEnumerable ToEnumerable(
                Object state, 
                ResolveContext<HarshProvisionerContext> context
            )
                => ImmutableArray.Create(_str);
        }

        private class IdResolveBuilder :
            IdentifierResolveBuilder<String, ClientObjectResolveContext, String>
        {
            public IdResolveBuilder(
                IResolveBuilder<String> parent, 
                IEnumerable<String> identifiers
            ) 
                : base(parent, identifiers)
            {
            }

            protected override String GetIdentifier(String result)
                => result;
        }
    }
}

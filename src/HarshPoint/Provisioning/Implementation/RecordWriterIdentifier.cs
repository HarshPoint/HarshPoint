using Microsoft.SharePoint.Client;
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace HarshPoint.Provisioning.Implementation
{
    class RecordWriterIdentifier
    {
        private static ImmutableDictionary<Type, Expression> CreateSelectors()
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, Expression>();

            return builder.ToImmutable();
        }

        private static readonly ImmutableDictionary<Type, Expression> _selectors
            = CreateSelectors();
    }
}

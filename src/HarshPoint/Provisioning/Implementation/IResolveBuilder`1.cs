﻿using System;
using System.Collections;

namespace HarshPoint.Provisioning.Implementation
{
    public interface IResolveBuilder<TContext> : IResolveBuilder
        where TContext : IResolveContext
    {
        void InitializeContext(TContext context);

        Object Initialize(TContext context);

        IEnumerable ToEnumerable(Object state, TContext context);
    }
}

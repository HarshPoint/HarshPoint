using System;
using HarshPoint.Provisioning.Output;

namespace HarshPoint.Provisioning
{
    public static class Result
    {
        public static HarshProvisionerOutput AlreadyExists<T>(String identifier, T @object)
            => new ObjectAlreadyExists<T>(identifier, @object);

        public static ObjectCreated<T> Created<T>(String identifier, T @object)
            => new ObjectCreated<T>(identifier, @object);
    }
}

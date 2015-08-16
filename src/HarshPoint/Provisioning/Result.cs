using HarshPoint.Provisioning.Output;
using System;

namespace HarshPoint.Provisioning
{
    public static class Result
    {
        public static ObjectAdded<T> Added<T>(String identifier, T @object)
            => Added(identifier, @object, null);

        public static ObjectAdded<T> Added<T>(String identifier, T @object, Object parent)
            => new ObjectAdded<T>(identifier, parent, @object);

        public static HarshProvisionerOutput AlreadyExists<T>(String identifier, T @object)
            => AlreadyExists(identifier, @object, null);

        public static HarshProvisionerOutput AlreadyExists<T>(String identifier, T @object, Object parent)
            => new ObjectAlreadyExists<T>(identifier, parent, @object);

        public static HarshProvisionerOutput DidNotExist(String identifier)
            => DidNotExist(identifier, null);

        public static HarshProvisionerOutput DidNotExist(String identifier, Object parent)
            => new ObjectDidNotExist(identifier, parent);

        public static HarshProvisionerOutput Removed(String identifier)
            => Removed(identifier, null);

        public static HarshProvisionerOutput Removed(String identifier, Object parent)
            => new ObjectRemoved(identifier, parent);
    }
}

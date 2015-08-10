using System;

namespace HarshPoint.Provisioning
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MandatoryWhenCreatingAttribute : Attribute
    {
    }
}

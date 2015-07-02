using System;

namespace HarshPoint.Provisioning
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class NeverDeletesUserDataAttribute : Attribute
    {
    }
}

using System;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public interface IDefaultValuePolicy
    {
        Boolean IsDefaultValue(Object value);
        Boolean SupportsType(TypeInfo typeInfo);
    }
}

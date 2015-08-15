using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public interface ITrackValueSource
    {
        PropertyValueSource GetValueSource(PropertyInfo propertyInfo);
        void SetValueSource(PropertyInfo propertyInfo, PropertyValueSource source);
    }
}

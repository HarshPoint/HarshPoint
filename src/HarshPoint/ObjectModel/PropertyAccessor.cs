using HarshPoint.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public sealed class PropertyAccessor
    {
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyInfo));
            }

            PropertyInfo = propertyInfo;

            Expression = propertyInfo.MakeGetterExpression(
                resultType: typeof(Object)
            );

            Getter = propertyInfo.MakeGetter();
            Setter = propertyInfo.MakeSetter();
        }

        public Expression Expression { get; }
        public PropertyInfo PropertyInfo { get; }

        private Func<Object, Object> Getter { get; }
        private Action<Object, Object> Setter { get; }

        public Type DeclaringType => PropertyInfo.DeclaringType;
        public String Name => PropertyInfo.Name;
        public Type PropertyType => PropertyInfo.PropertyType;
        public TypeInfo PropertyTypeInfo => PropertyType.GetTypeInfo();

        public Object GetValue(Object target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            return Getter(target);
        }

        public PropertyValueSource GetValueSource(ITrackValueSource target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            return target.GetValueSource(PropertyInfo);
        }

        public void SetValue(Object target, Object value)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            Setter(target, value);

            var trackSource = (target as ITrackValueSource);
            if (trackSource != null)
            {
                SetValueSource(trackSource, null);
            }
        }

        public void SetValue(ITrackValueSource target, Object value, PropertyValueSource source)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            Setter(target, value);
            SetValueSource(target, source);
        }

        public override String ToString() => PropertyInfo.ToString();

        private void SetValueSource(ITrackValueSource target, PropertyValueSource source)
        {
            target.SetValueSource(PropertyInfo, source);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<PropertyAccessor>();
    }
}

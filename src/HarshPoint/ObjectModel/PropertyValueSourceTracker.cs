using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.ObjectModel
{
    public sealed class PropertyValueSourceTracker : ITrackValueSource
    {
        private readonly Dictionary<PropertyInfo, PropertyValueSource> _sources
            = new Dictionary<PropertyInfo, PropertyValueSource>();

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public PropertyValueSource GetValueSource(Expression<Func<Object>> expression)
        {
            if (expression == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(expression));
            }

            return GetValueSource(
                expression.ExtractLastPropertyAccess()
            );
        }

        public PropertyValueSource GetValueSource(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyInfo));
            }

            PropertyValueSource result = null;
            _sources.TryGetValue(propertyInfo, out result);
            return result;
        }

        public void SetValueSource(PropertyInfo propertyInfo, PropertyValueSource source)
        {
            if (propertyInfo == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(propertyInfo));
            }

            _sources[propertyInfo] = source;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<PropertyValueSourceTracker>();
    }
}

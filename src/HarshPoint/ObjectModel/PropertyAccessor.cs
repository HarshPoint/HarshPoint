﻿using HarshPoint.Reflection;
using System;
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
            Getter = propertyInfo.MakeGetter();
            Setter = propertyInfo.MakeSetter();
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public Action<Object, Object> Setter { get; private set; }
        public Func<Object, Object> Getter { get; private set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext<PropertyAccessor>();
    }
}

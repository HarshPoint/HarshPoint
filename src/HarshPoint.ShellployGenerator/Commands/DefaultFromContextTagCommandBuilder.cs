using HarshPoint.Provisioning;
using HarshPoint.ShellployGenerator.Builders;
using System;
using SMA = System.Management.Automation;

namespace HarshPoint.ShellployGenerator.Commands
{
    public abstract class DefaultFromContextTagCommandBuilder<TTag> : 
        NewObjectCommandBuilder<TTag>
        where TTag : IDefaultFromContextTag
    {
        protected DefaultFromContextTagCommandBuilder()
        {
            Aliases.Add(TagValueType.Name);
            PropertyContainer.Update(ValuePropertyName, ValueProperty);
        }
        
        private static Type GetTagValueType()
        {
            var baseType = typeof(TTag).BaseType;

            if (baseType == null)
            {
                return null;
            }

            if (!baseType.IsGenericType)
            {
                return null;
            }

            if (baseType.GetGenericTypeDefinition()
                != typeof(DefaultFromContextTag<>))
            {
                return null;
            }

            return baseType.GetGenericArguments()[0];
        }

        private const String ValuePropertyName = "Value";

        private static readonly Type TagValueType = GetTagValueType();

        private static readonly PropertyModel ValueProperty
            = new PropertyModelAssignedTo(
                ValuePropertyName,
                new PropertyModelSynthesized(
                    ValuePropertyName,
                    TagValueType,
                    new AttributeModel(typeof(SMA.ParameterAttribute))
                        .SetProperty("Mandatory", true)
                        .SetProperty("Position", 0)
                        .SetProperty("ValueFromPipeline", true)
                )
            );
    }
}

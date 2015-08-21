using System;

namespace HarshPoint.ShellployGenerator.Builders
{
    internal sealed class ParameterBuilderDefaultValue : ParameterBuilder
    {
        internal ParameterBuilderDefaultValue(Object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public Object DefaultValue { get; }

        protected override void Process(ShellployCommandProperty property)
        {
            property.DefaultValue = DefaultValue;
        }

        internal override ParameterBuilder WithNext(ParameterBuilder next)
        {
            if (next?.HasElementOfType<ParameterBuilderFixed>() ?? false)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ParameterBuilderDefaultValue_AttemptedToNestFixed
                );
            }

            return base.WithNext(next);
        }


        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ParameterBuilderDefaultValue));
    }
}

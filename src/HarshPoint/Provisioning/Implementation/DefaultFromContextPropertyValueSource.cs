using HarshPoint.ObjectModel;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class DefaultFromContextPropertyValueSource : PropertyValueSource
    {
        private DefaultFromContextPropertyValueSource() { }

        public static PropertyValueSource Instance { get; }
            = new DefaultFromContextPropertyValueSource();
    }
}
using HarshPoint.ObjectModel;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.ObjectModel
{
    public class PropertyValueSourceTracking : SeriloggedTest
    {
        public PropertyValueSourceTracking(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void PropertySource_is_stored_and_retrieved()
        {
            var target = new Tracker();
            var source = new TestSource();
            var metadata = new HarshObjectMetadata(typeof(Tracker));

            var accessor = metadata.ModelProperties.Single();

            accessor.SetValue(target, "42", source);

            Assert.Equal("42", target.Property);
            Assert.Same(source, accessor.GetValueSource(target));
        }

        private sealed class Tracker : ITrackValueSource
        {
            private readonly PropertyValueSourceTracker _tracker
                = new PropertyValueSourceTracker();

            public String Property
            {
                get; set;
            }

            public PropertyValueSource GetValueSource(PropertyInfo propertyInfo)
                => _tracker.GetValueSource(propertyInfo);

            public void SetValueSource(PropertyInfo propertyInfo, PropertyValueSource source)
            {
                _tracker.SetValueSource(propertyInfo, source);
            }
        }

        private sealed class TestSource : PropertyValueSource { }
    }
}

using System;
using Xunit;

namespace HarshPoint.Tests
{
    public class CloneableTests
    {
        [Fact]
        public void With_returns_a_clone()
        {
            var x = new DummyCloneable()
            {
                Field = "42",
                Property = 4242,
            };

            var y = x.With(c => c.Property += 1);

            Assert.NotSame(x, y);
            Assert.Equal(x.Field, y.Field);
            Assert.NotEqual(x.Property, y.Property);
        }


        [Fact]
        public void With_returns_original_value_if_no_property_modification()
        {
            var x = new DummyCloneable()
            {
                Field = "42",
                Property = 4242,
            };

            var y = x.With(c => c.Property, x.Property);
            Assert.Same(x, y);
        }

        [Fact]
        public void With_returns_original_value_if_no_field_modification()
        {
            var x = new DummyCloneable()
            {
                Field = "42",
                Property = 4242,
            };

            var y = x.With(c => c.Field, x.Field);
            Assert.Same(x, y);
        }

        [Fact]
        public void With_returns_clone_if_field_value_differs()
        {
            var x = new DummyCloneable()
            {
                Field = "42",
                Property = 4242,
            };

            var y = x.With(c => c.Field, "5252");

            Assert.NotSame(x, y);
            Assert.Equal("42", x.Field);
            Assert.Equal("5252", y.Field);
        }

        [Fact]
        public void With_returns_clone_if_property_value_differs()
        {
            var x = new DummyCloneable()
            {
                Field = "42",
                Property = 4242,
            };

            var y = x.With(c => c.Property, 52);

            Assert.NotSame(x, y);
            Assert.Equal(4242, x.Property);
            Assert.Equal(52, y.Property);
        }


        private class DummyCloneable : IHarshCloneable
        {
            public String Field;

            public Int32 Property
            {
                get;
                set;
            }

            public Object Clone() => MemberwiseClone();
        }
    }
}

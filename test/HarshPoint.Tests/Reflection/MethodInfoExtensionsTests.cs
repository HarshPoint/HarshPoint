using HarshPoint.Reflection;
using System;
using Xunit;

namespace HarshPoint.Tests.Reflection
{
    public class MethodInfoExtensionsTests
    {
        [Fact]
        public void OverrideChain_return_methods_in_order_from_most_derived_to_base()
        {
            var expected = new[]
            {
                typeof(C).GetMethod("M"),
                typeof(B).GetMethod("M"),
                typeof(A).GetMethod("M")
            };

            var actual = typeof(A).GetMethod("M").GetRuntimeBaseMethodChain(typeof(C));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OverrideChain_returns_only_method_itself_if_not_overriden()
        {
            var bNotVirtual = typeof(B).GetMethod("NotVirtual");

            var cNotVirtual = typeof(C).GetMethod("NotVirtual");

            Assert.Equal(new[] { bNotVirtual }, bNotVirtual.GetRuntimeBaseMethodChain(typeof(C)));
            Assert.Equal(new[] { cNotVirtual }, cNotVirtual.GetRuntimeBaseMethodChain(typeof(C)));
        }

        [Fact(DisplayName = "MakeSetter lambda calls an internal setter.")]
        public void MakeSetter_calls_an_internal_setter()
        {
            var prop = typeof(InternalSetterType).GetProperty("Property");
            var setter = prop.MakeSetter<Object, Object>();

            var target = new InternalSetterType();
            setter(target, "42");

            Assert.Equal("42", target.Property);
        }

        private class InternalSetterType
        {
            public String Property
            {
                get;
                internal set;
            }
        }

        private class A
        {
            public virtual void M() { }
        }

        private class B : A
        {
            public void NotVirtual() { }
            public override void M() { }
        }

        private class C : B
        {
            public new void NotVirtual() { }
            public override void M() { }
        }
    }
}

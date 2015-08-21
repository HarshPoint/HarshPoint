using HarshPoint.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HarshPoint.Tests.ObjectModel
{
    public class ChainTests
    {
        [Fact]
        public void Calls_self_via_element_interface()
        {
            var chain = new TestChain("42");
            Assert.Single(chain.GetValues(), "42");
        }

        [Fact]
        public void Calls_self_and_next_via_element_interface()
        {
            var first = new TestChain("42");
            var next = new TestChain("4242");

            var chain = first.And(next);

            Assert.Equal(new[] { "42", "4242" }, chain.GetValues());
        }

        [Fact]
        public void Concatenates_two_chains()
        {
            var first = new TestChain("1").And(new TestChain("2"));
            var second = new TestChain("3").And(new TestChain("4"));

            Assert.Equal(new[] { "1", "2" }, first.GetValues());
            Assert.Equal(new[] { "3", "4" }, second.GetValues());

            var result = first.And(second);

            Assert.NotSame(first, result);
            Assert.Equal(new[] { "1", "2", "3", "4" }, result.GetValues());
        }

        [Fact]
        public void Cannot_add_self_to_chain()
        {
            var x = new TestChain("1");

            Assert.Throws<ArgumentException>(
                () => x.And(x)
            );
        }

        [Fact]
        public void Cannot_add_existing_element_to_chain()
        {
            var one = new TestChain("1");
            var two = new TestChain("2");

            var zero = new TestChain("0").And(one).And(two);

            Assert.Throws<ArgumentException>(
                () => zero.And(zero.Next)
            );
        }

        private sealed class TestChain : Chain<ITestChainElement>, ITestChainElement
        {
            private readonly String _value;

            public TestChain(String value)
            {
                _value = value;
            }

            public TestChain And(TestChain other) => (TestChain)Append(other);

            public TestChain Next => (TestChain)Elements.Skip(1).FirstOrDefault();

            public IEnumerable<String> GetValues()
                => Elements.Select(e => e.GetValue());

            String ITestChainElement.GetValue() => _value;
        }

        private interface ITestChainElement
        {
            String GetValue();
        }
    }
}

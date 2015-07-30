using System;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests
{
    public class DependencyGraphTests : SeriloggedTest
    {
        public DependencyGraphTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Returns_empty_sequence()
        {
            Assert.Empty(HarshDependencyGraph<String>.Empty.Sort());
        }

        [Fact]
        public void Returns_one_item()
        {
            Assert.Single(HarshDependencyGraph<String>.Empty.AddNode("42").Sort(), "42");
        }

        [Fact]
        public void Returs_dependencies_before_item()
        {
            var dag = HarshDependencyGraph<String>.Empty
                .AddEdge("item", "dep1")
                .AddEdge("item", "dep2")
                .AddEdge("dep2", "dep1");

            Assert.Equal(new[] { "dep1", "dep2", "item" }, dag.Sort());
        }
    }
}

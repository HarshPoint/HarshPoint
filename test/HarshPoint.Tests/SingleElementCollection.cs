using Xunit;

namespace HarshPoint.Tests
{
    public class SingleElementCollection
    {
        [Fact]
        public void Returns_single_value()
        {
            var enumerable = HarshSingleElementCollection.Create(42);
            Assert.Single(enumerable, 42);
        }
    }
}

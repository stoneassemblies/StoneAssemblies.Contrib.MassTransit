namespace StoneAssemblies.Contrib.MassTransit.Tests.Services
{
    using System;
    using System.Threading.Tasks;

    using global::MassTransit;

    using Moq;

    using StoneAssemblies.Contrib.MassTransit.Services;

    using Xunit;

    public class BusSelectorPredicateFacts
    {
        public class The_Constructor_Method
        {
            [Fact]
            public async Task Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new BusSelectorPredicate<DemoMessage>(null));
            }

            [Fact]
            public async Task Succeeds_With_None_Null_Arguments()
            {
                var busSelector = new BusSelectorPredicate<DemoMessage>(async (bus, message) => true);
                Assert.NotNull(busSelector);
            }
        }

        public class The_IsMatch_Method
        {
            [Fact]
            public async Task Returns_False_If_The_Predicate_Returns_False()
            {
                var busSelectorPredicate = new BusSelectorPredicate<DemoMessage>(async (bus, message) => false);
                var isMatch = await busSelectorPredicate.IsMatch(new Mock<IBus>().Object, new DemoMessage());
                Assert.False(isMatch);
            }

            [Fact]
            public async Task Returns_True_If_The_Predicate_Returns_True()
            {
                var busSelectorPredicate = new BusSelectorPredicate<DemoMessage>(async (bus, message) => true);
                var isMatch = await busSelectorPredicate.IsMatch(new Mock<IBus>().Object, new DemoMessage());
                Assert.True(isMatch);
            }
        }
    }
}
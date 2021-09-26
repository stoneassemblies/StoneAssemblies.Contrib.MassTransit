namespace StoneAssemblies.Contrib.MassTransit.Tests.Services
{
    using System;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using global::MassTransit;

    using Moq;

    using StoneAssemblies.Contrib.MassTransit.Services;

    using Xunit;

    public class DefaultBusSelectorFacts
    {
        public class The_Constructor_Method
        {
            [Fact]
            public async Task Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new DefaultBusSelector<DemoMessage>(null));
            }

            [Fact]
            public async Task Succeeds_With_None_Null_Arguments()
            {
                var defaultBusSelector = new DefaultBusSelector<DemoMessage>(new Mock<IBus>().Object);
                Assert.NotNull(defaultBusSelector);
            }
        }

        public class The_SelectClientFactories_Method
        {
            [Fact]
            public async Task Returns_The_ClientFactory()
            {
                var defaultBusSelector = new DefaultBusSelector<DemoMessage>(new Mock<IBus>().Object);
                var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();
                Assert.NotEmpty(clientFactories);
            }
        }
    }
}
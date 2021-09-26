namespace StoneAssemblies.Contrib.MassTransit.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Dasync.Collections;

    using global::MassTransit;

    using Moq;

    using StoneAssemblies.Contrib.MassTransit.Services;

    using Xunit;

    public class PredicateBasedBusSelector
    {
        public class The_Constructor_Method
        {
            [Fact]
            public async Task Succeeds_With_None_Null_Arguments()
            {
                var busSelector = new PredicateBasedBusSelector<DemoMessage>(
                    new List<IBus>
                        {
                            new Mock<IBus>().Object
                        },
                    new BusSelectorPredicate<DemoMessage>(async (bus, message) => true));

                Assert.NotNull(busSelector);
            }

            [Fact]
            public async Task Throws_ArgumentNullException_If_Buses_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new PredicateBasedBusSelector<DemoMessage>(
                        null,
                        new BusSelectorPredicate<DemoMessage>(async (bus, message) => true)));
            }

            [Fact]
            public async Task Throws_ArgumentNullException_If_Predicate_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => new PredicateBasedBusSelector<DemoMessage>(null, null));
            }
        }
    }

    public class The_SelectClientFactories_Method
    {
        [Fact]
        public async Task Does_Not_Return_The_ClientFactories_If_Predicate_Returns_False()
        {
            var bus = new Mock<IBus>().Object;

            var defaultBusSelector = new PredicateBasedBusSelector<DemoMessage>(
                new List<IBus>
                    {
                        bus
                    },
                new BusSelectorPredicate<DemoMessage>(async (b, m) => false));

            var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();

            Assert.Empty(clientFactories);
        }

        [Fact]
        public async Task Returns_The_ClientFactory()
        {
            var bus = new Mock<IBus>().Object;

            var defaultBusSelector = new PredicateBasedBusSelector<DemoMessage>(
                new List<IBus>
                    {
                        bus
                    },
                new BusSelectorPredicate<DemoMessage>(async (b, m) => true));
            var clientFactories = await defaultBusSelector.SelectClientFactories(new DemoMessage()).ToListAsync();
            Assert.NotEmpty(clientFactories);
        }
    }
}
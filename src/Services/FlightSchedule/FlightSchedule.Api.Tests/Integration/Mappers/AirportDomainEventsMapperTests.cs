using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Event;
using FlightSchedule.Api.Integration.Events;
using FlightSchedule.Api.Integration.Mappers;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.Tests.Integration.Mappers
{
    public class AirportDomainEventsMapperTests
    {
        protected interface IMyAggregate : IAggregate
        {
        }

        private AggregateDomainEvent<IAggregate> Create(IAggregate current, IAggregate? prev, IEnumerable<IDomainEvent<IAggregate>> domainEvents)
        {
            var genericType = typeof(AggregateDomainEvent<>).MakeGenericType(current.GetType());
            return (AggregateDomainEvent<IAggregate>)Activator.CreateInstance(genericType, current, prev, domainEvents)!;
        }

        private struct AggregateDomainEvent<T>
        {
            public AggregateDomainEvent(T aggregate, T? oldAggregate, IEnumerable<IDomainEvent<T>> domainEvents)
            {
                Aggregate = aggregate;
                OldAggregate = oldAggregate;
                DomainEvents = domainEvents;
            }

            public T Aggregate { get; }
            public T? OldAggregate { get; }

            public IEnumerable<IDomainEvent<T>> DomainEvents { get; }


        }

        private class Dummy: AggregateRoot<Dummy, string>
        {
            protected override void When(IDomainEvent<Dummy> @event)
            {
                throw new NotImplementedException();
            }

            protected override void EnsureValidState()
            {
                throw new NotImplementedException();
            }
        }

        public static Type AggregateRootType = typeof(AggregateRoot<,>);

        public static IEnumerable<IDomainEvent<IAggregate>>? GetDomainEvents(object obj)
        {
            https://lostechies.com/patricklioi/2013/12/13/nailing-down-generics/
            var mi = obj.GetType().GetMethods().FirstOrDefault(t => t.Name == nameof(Dummy.GetDomainEvents));
            return (IEnumerable<IDomainEvent<IAggregate>>?)mi?.Invoke(obj, Array.Empty<object>());
        }

        bool IsAggregateRoot(object obj)
        {
            for (var type = obj.GetType().BaseType; type != null; type=type.BaseType)
            {
                if (AggregateRootType.Name == type.Name
                    && AggregateRootType.Namespace == type.Namespace
                    && AggregateRootType.Assembly == type.Assembly)
                {
                    return true;
                }
            }
            return false;
        }

        [Fact()]
        public void Should_Be_AggregateRoot()
        {

            var airport = Airport.Create(new IataLocationCode("YYY"), new ObjectName("Name"));
            IsAggregateRoot(new Dummy()).Should().BeTrue();
            IsAggregateRoot(airport).Should().BeTrue();
            // ReSharper disable once SuspiciousTypeConversion.Global
            var domainEvents = GetDomainEvents(new Dummy());
            domainEvents = GetDomainEvents(airport);
            domainEvents = airport.GetDomainEvents();
        }

        [Fact()]
        public void Should_Map()
        {
            var airport = Airport.Create(new IataLocationCode("YYY"), new ObjectName("Name"));
            var oldAirport = Airport.Create(new IataLocationCode("XXX"), new ObjectName("Name"));
            airport.SetIataCode(oldAirport.IataCode);
            var domainEvent = airport.GetDomainEvents().LastOrDefault();
            domainEvent.Should().NotBeNull();
            domainEvent.Should().BeOfType<Airport.Events.IataCodeUpdated>();
            var mapper = new AirportDomainEventsMapper();
            var integrationEvent = mapper.Project(airport, oldAirport, (IDomainEvent<Airport>)domainEvent!).FirstOrDefault();
            integrationEvent.Should().NotBeNull();
            integrationEvent.Should().BeOfType<AirportIataCodeChanged>();
            var airportIataCodeChangedEvent = (AirportIataCodeChanged)integrationEvent!;
            airportIataCodeChangedEvent.AirportId.Should().Be((Guid)airport.Id);
            airportIataCodeChangedEvent.NewCode.Should().Be((string)airport.IataCode);
            airportIataCodeChangedEvent.OldCode.Should().Be((string)oldAirport.IataCode);
        }
    }
}
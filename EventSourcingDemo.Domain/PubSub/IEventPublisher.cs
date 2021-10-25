using EventSourcingDemo.Domain.Base;
using System.Threading.Tasks;

namespace EventSourcingDemo.Domain.PubSub
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : IEvent;
    }
}

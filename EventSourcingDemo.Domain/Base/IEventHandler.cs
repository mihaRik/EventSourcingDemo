using System.Threading.Tasks;

namespace EventSourcingDemo.Domain.Base
{
    public interface IEventHandler<T> where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}

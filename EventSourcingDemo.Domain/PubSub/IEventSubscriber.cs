using EventSourcingDemo.Domain.Base;
using System;
using System.Threading.Tasks;

namespace EventSourcingDemo.Domain.PubSub
{
    public interface IEventSubscriber
    {
        void Subscribe<T>(Action<T> handler) where T : IEvent;

        void Subscribe<T>(Func<T, Task> handler) where T : IEvent;
    }
}

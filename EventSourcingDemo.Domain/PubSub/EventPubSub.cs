
using EventSourcingDemo.Domain.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDemo.Domain.PubSub
{
    public class EventPubSub : IEventPublisher, IEventSubscriber
    {
        private static AsyncLocal<Dictionary<Type, List<object>>> handlers = new();

        public Dictionary<Type, List<object>> Handlers
        {
            get => handlers.Value ??= new Dictionary<Type, List<object>>();
        }

        public async Task PublishAsync<T>(T @event) where T : IEvent
        {
            foreach (var handler in GetHandlersOf<T>())
            {
                switch (handler)
                {
                    case Action<T> action:
                        action(@event);
                        break;
                    case Func<T, Task> action:
                        await action(@event);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            GetHandlersOf<T>().Add(handler);
        }

        public void Subscribe<T>(Func<T, Task> handler) where T : IEvent
        {
            GetHandlersOf<T>().Add(handler);
        }

        private ICollection<object> GetHandlersOf<T>()
        {
            return Handlers.GetValueOrDefault(typeof(T)) ?? (Handlers[typeof(T)] = new List<object>());
        }
    }
}

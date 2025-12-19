using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkDirectX11.Events
{
    public class EventAggregator
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (!_subscribers.ContainsKey(typeof(TEvent)))
            {
                _subscribers[typeof(TEvent)] = new List<object>();
            }
            _subscribers[typeof(TEvent)].Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                _subscribers[typeof(TEvent)].Remove(handler);
                if (_subscribers[typeof(TEvent)].Count == 0)
                {
                    _subscribers.Remove(typeof(TEvent));
                }
            }
        }

        public void Publish<TEvent>(TEvent eventData)
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                foreach (var handler in _subscribers[typeof(TEvent)])
                {
                    (handler as Action<TEvent>)?.Invoke(eventData);
                }
            }
        }
    }

    public static class EventAggregatorProvider
    {
        public static EventAggregator Instance { get; } = new();
    }
}

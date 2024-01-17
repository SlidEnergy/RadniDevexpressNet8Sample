using System;
using System.Collections.Generic;

namespace CommonBlazor.Infrastructure
{
    public class Messenger : IMessenger
    {
        private Dictionary<Type, Dictionary<object, object>> _registeredMessages = new Dictionary<Type, Dictionary<object, object>>();
        public object _lockObject = new object();

        public Messenger()
        {

        }

        public void Register<TMessage>(object source, Action<TMessage> handler) where TMessage : class
        {
            var type = typeof(TMessage);

            lock (_lockObject)
            {
                if (!_registeredMessages.TryGetValue(type, out var list))
                {
                    list = new Dictionary<object, object>();
                    _registeredMessages.TryAdd(type, list);
                }

                list.Add(source, handler);
            }
        }

        /// <summary>
        /// Unregister of all handlers by source
        /// </summary>
        public void UnregisterAll(object source)
        {
            lock (_lockObject)
            {
                foreach (var handlers in _registeredMessages.Values)
                {
                    if (handlers.ContainsKey(source))
                    {
                        handlers.Remove(source);
                    }
                }
            }
        }

        /// <summary>
        /// Unregister of all handlers of source by message type.
        /// </summary>
        public void Unregister<TMessage>(object source) where TMessage : class
        {
            var type = typeof(TMessage);

            lock (_lockObject)
            {
                if (_registeredMessages.TryGetValue(type, out var handlers))
                {
                    if (handlers.ContainsKey(source))
                    {
                        handlers.Remove(source);
                    }
                }
            }
        }

        public void Send<TMessage>(TMessage message) where TMessage : class
        {
            var type = typeof(TMessage);

            lock (_lockObject)
            {
                if (_registeredMessages.TryGetValue(type, out var list))
                {
                    foreach (var item in list.Values)
                    {
                        var handler = item as Action<TMessage>;

                        handler?.Invoke(message);
                    }

                }
            }
        }
    }
}

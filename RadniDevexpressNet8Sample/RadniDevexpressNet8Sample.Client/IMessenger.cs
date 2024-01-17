using System;

namespace CommonBlazor.Infrastructure
{
    public interface IMessenger
    {
        void Register<TMessage>(object source, Action<TMessage> handler) where TMessage : class;
        void Send<TMessage>(TMessage message) where TMessage : class;
        void UnregisterAll(object source);
        void Unregister<TMessage>(object source) where TMessage : class;
    }
}
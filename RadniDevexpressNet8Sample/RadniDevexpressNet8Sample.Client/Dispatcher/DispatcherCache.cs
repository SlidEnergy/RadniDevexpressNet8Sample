using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    internal class DispatcherCache
    {
        private static readonly object _initializeCacheLock;
        private readonly IEnumerable<Type> _requestHandlerTypes;

        internal ConcurrentDictionary<Type, Element> Elements { get; }

        static DispatcherCache()
        {
            _initializeCacheLock = new object();
        }

        internal sealed class Element
        {
            public Type RequestType { get; set; }
            public Type ResponseType { get; set; }
            public Type HandlerType { get; set; }

            public Element(Type requestType, Type responseType, Type handlerType)
            {
                RequestType = requestType;
                ResponseType = responseType;

                var genericArguments = RequestType.GetGenericArguments();
                if (handlerType.ContainsGenericParameters && genericArguments.Count() > 0)
                    handlerType = handlerType.MakeGenericType(genericArguments);

                HandlerType = handlerType;
            }
        }

        public DispatcherCache(IEnumerable<Type> requestHandlerTypes, IEnumerable<Type> requestsTypes)
        {
            lock (_initializeCacheLock)
            {
                _requestHandlerTypes = requestHandlerTypes;
                Elements = new ConcurrentDictionary<Type, Element>();

                foreach (var requestType in requestsTypes)
                {
                    Elements.TryAdd(requestType, ConstructElement(requestType));
                }
            }
        }

        public Element ConstructRuntimeElement(Type requestType)
        {
            var element = ConstructElement(requestType);
            Elements.TryAdd(requestType, element);

            return element;
        }

        private Element ConstructElement(Type requestType)
        {
            var requestInterfaceType = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IRequest<>)));

            var responseType = requestInterfaceType.GetGenericArguments()[0];
            var handlerType = _requestHandlerTypes.FirstOrDefault(x => x.GetInterfaces()
                .Any(y => y.IsGenericType && y.GetGenericArguments()[0].Name == requestType.Name));

            if (handlerType is null)
                return null;

            return new Element(requestType, responseType, handlerType);
        }
    }
}

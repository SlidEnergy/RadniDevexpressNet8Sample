using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    internal sealed class DispatcherService : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatcherCache _cache;

        public DispatcherService(IServiceProvider serviceProvider, DispatcherCache dispatcherCache)
        {
            _serviceProvider = serviceProvider;
            _cache = dispatcherCache;
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return InitializeOperation(request).Process(cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return InitializeOperation(request).Process<TResponse>(cancellationToken);
        }

        private DispatcherOperation InitializeOperation(object request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            _cache.Elements.TryGetValue(requestType, out DispatcherCache.Element element);

            if (element is null)
            {
                element = _cache.ConstructRuntimeElement(requestType);
            }

            var processor = (DispatcherProcessor)Activator.CreateInstance(typeof(DispatcherProcessor<,>).MakeGenericType(element.RequestType, element.ResponseType));

            return new DispatcherOperation(_serviceProvider, processor, element.HandlerType, request);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    internal abstract class DispatcherProcessor
    {
        public abstract Task<object> ProcessUndefined(DispatcherOperation operation, CancellationToken cancellationToken);
    }

    internal abstract class DispatcherProcessor<TResponse> : DispatcherProcessor
    {
        public abstract Task<TResponse> ProcessDefined(DispatcherOperation operation, CancellationToken cancellationToken);
    }

    internal sealed class DispatcherProcessor<TRequest, TResponse> : DispatcherProcessor<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override async Task<object> ProcessUndefined(DispatcherOperation operation, CancellationToken cancellationToken)
        {
            return await ProcessDefined(operation, cancellationToken);
        }

        public override Task<TResponse> ProcessDefined(DispatcherOperation operation, CancellationToken cancellationToken)
        {
            var serviceProvider = operation.ServiceProvider;
            var handlerType = operation.HandlerType;
            var request = operation.Request;

            Task<TResponse> Handler() =>
                ((IRequestHandler<TRequest, TResponse>)serviceProvider.ConstructInstance(handlerType)).Handle((TRequest)request, cancellationToken);

            return Handler();
        }
    }
}

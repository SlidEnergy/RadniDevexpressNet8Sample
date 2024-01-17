using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    internal class DispatcherOperation
    {
        public IServiceProvider ServiceProvider { get; }
        public DispatcherProcessor Processor { get; }
        public Type HandlerType { get; }
        public object Request { get; }

        public DispatcherOperation(
            IServiceProvider serviceProvider, DispatcherProcessor processor, Type handlerType, object request)
        {
            ServiceProvider = serviceProvider;
            HandlerType = handlerType;
            Processor = processor;
            Request = request;
        }

        public Task<object> Process(CancellationToken cancellationToken)
        {
            return Processor.ProcessUndefined(this, cancellationToken);
        }

        public Task<TResponse> Process<TResponse>(CancellationToken cancellationToken)
        {
            var processor = (DispatcherProcessor<TResponse>)Processor;

            return processor.ProcessDefined(this, cancellationToken);
        }
    }
}

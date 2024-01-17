using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    public interface IRequestHandler
    {
    }

    public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, bool>
        where TRequest : IRequest
    {
    }
}

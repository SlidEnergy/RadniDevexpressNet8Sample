using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure.Dispatcher
{
    public interface IDispatcher
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task<object> Send(object request, CancellationToken cancellationToken = default);
    }
}


namespace CommonBlazor.Infrastructure.Dispatcher
{
    public interface IRequest<out TResponse>
    {
    }

    public interface IRequest : IRequest<bool>
    {
    }
}

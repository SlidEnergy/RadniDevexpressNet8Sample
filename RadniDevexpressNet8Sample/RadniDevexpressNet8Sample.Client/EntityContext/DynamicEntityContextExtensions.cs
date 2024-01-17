using CommonBlazor.Infrastructure;

namespace CommonBlazor.DynamicData
{
    public static class DynamicEntityContextExtensions
    {
        public static async Task InitializeAsync(this DynamicEntityContext context, CancellationToken cancellationToken = default)
        {
            var initializer = ServiceResolver.Resolve<DynamicEntityContextInitializer>();

            await initializer.InitializeContextAsync(context, cancellationToken);
        }
    }
}

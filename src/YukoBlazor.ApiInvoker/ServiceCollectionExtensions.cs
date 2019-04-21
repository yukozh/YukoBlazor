using YukoBlazor.ApiInvoker;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClient(this IServiceCollection self)
        {
            return self
                .AddSingleton<IdentityContainer>()
                .AddSingleton<ApiClient>();
        }
    }
}

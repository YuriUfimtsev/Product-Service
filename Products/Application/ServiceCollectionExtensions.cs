using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddRepositories();
        serviceCollection.AddSingleton<IProductService, ProductService>();

        return serviceCollection;
    }
}
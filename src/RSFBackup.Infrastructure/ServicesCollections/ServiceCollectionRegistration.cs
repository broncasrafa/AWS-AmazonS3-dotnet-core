using Microsoft.Extensions.DependencyInjection;
using RSFBackup.Core.Interfaces.Repositories;
using RSFBackup.Infrastructure.Repositories;

namespace RSFBackup.Infrastructure.ServicesCollections;

public static class ServiceCollectionRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBucketRepository, BucketRepository>();
        services.AddSingleton<IFilesRepository, FilesRepository>();
    }
}

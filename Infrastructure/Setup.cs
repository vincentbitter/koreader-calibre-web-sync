using Sync.Domain.Repositories;
using Sync.Infrastructure.Repositories;
using Sync.Infrastructure.Services;

namespace Sync.Infrastructure;

public static class Setup
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<CalibreWebService>();
    }
}
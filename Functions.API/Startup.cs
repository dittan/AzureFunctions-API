using Functions.Core.Interfaces.Repositorys;
using Functions.Infra.Repositorys;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.API.Startup))]

namespace Functions.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Repositorys
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
        }
    }
}

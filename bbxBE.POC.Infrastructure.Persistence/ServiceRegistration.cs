using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Contexts;
using bbxBE.POC.Infrastructure.Persistence.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bbxBE.POC.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            #region Repositories

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IPositionRepositoryAsync, PositionRepositoryAsync>();
            services.AddTransient<IEmployeeRepositoryAsync, EmployeeRepositoryAsync>();

            #endregion Repositories
        }
    }
}
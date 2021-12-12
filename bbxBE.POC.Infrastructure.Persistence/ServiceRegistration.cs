using bbxBE.POC.Application.Interfaces.Repositories;
using bbxBE.POC.Infrastructure.Persistence.Contexts;
using bbxBE.POC.Infrastructure.Persistence.Query;
using bbxBE.POC.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bbxBE.POC.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DapperContext>();

            //if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseInMemoryDatabase("ApplicationDb"));
            //}
            //else
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlServer(
            //       configuration.GetConnectionString("DefaultConnection"),
            //       b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            //}

            #region Repositories

            services.AddSingleton<IProductQueryRepository, ProductQueryRepository>();

            #endregion Repositories

            #region Commands And Queries

            services.AddSingleton<ProductListQuery>();

            #endregion Commands And Queries
        }
    }
}
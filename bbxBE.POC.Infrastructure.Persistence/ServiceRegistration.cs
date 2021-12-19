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

            #region Repositories

            services.AddSingleton<IProductQueryRepository, ProductQueryRepository>();

            #endregion Repositories

            #region Commands And Queries

            services.AddSingleton<ProductListQueryForSearch>();
            services.AddSingleton<ProductSearchQuery>();
            services.AddSingleton<ProductFindQuery>(); 

            #endregion Commands And Queries
        }
    }
}
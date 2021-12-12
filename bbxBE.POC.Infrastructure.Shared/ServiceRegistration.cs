using bbxBE.POC.Application.Interfaces;
using bbxBE.POC.Domain.Settings;
using bbxBE.POC.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bbxBE.POC.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<ReportSettings>(_config.GetSection("ReportSettings"));
            //services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IReportService, ReportService>();
        }
    }
}
using MenuApp.BLL.Services.ReceiptService;
using MenuApp.BLL.Services.SubscriptionService;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class ReceiptServiceExtension
    {
        public static IServiceCollection AddReceiptService(this IServiceCollection services)
        {
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IReceipesRepository, ReceipesRepository>();

            return services;
        }
    }
}

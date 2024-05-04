using MenuApp.BLL.Services.SubscriptionService;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class SubscriptionServiceExtension
    {
        public static IServiceCollection AddSubscriptionService(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return services;
        }
    }
}

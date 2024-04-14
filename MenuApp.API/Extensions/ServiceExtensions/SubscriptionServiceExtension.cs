using MenuApp.BLL.Services.SubscriptionService;
using MenuApp.BLL.Services.UserService;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

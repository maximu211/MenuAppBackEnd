using MenuApp.BLL.Services.RecipeService;
using MenuApp.BLL.Services.SubscriptionService;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class RecipeServiceExtension
    {
        public static IServiceCollection AddRecipeService(this IServiceCollection services)
        {
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IReceipeRepository, RecipeRepository>();

            return services;
        }
    }
}

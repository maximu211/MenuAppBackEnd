﻿using MenuApp.BLL.Services.RecipeService;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class RecipeServiceExtension
    {
        public static IServiceCollection AddRecipeService(this IServiceCollection services)
        {
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();

            return services;
        }
    }
}

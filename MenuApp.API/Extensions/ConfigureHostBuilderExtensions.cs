using MenuApp.BLL.Services.UserService;
using MenuApp.DAL.Repositories;
using static MenuApp.BLL.Services.UserService.UserService;

namespace MenuApp.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUsersRepository, UserRepository>();
            services.AddSingleton<IGenerateJwtToken, GenerateJwtToken>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}

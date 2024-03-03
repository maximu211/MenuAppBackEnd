using MenuApp.BLL.Services.UserService;
using MenuApp.BLL.Utils;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions
{
    public static class UserServiceExtensions
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

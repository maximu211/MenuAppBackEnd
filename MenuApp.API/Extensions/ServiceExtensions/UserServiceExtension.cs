using MenuApp.BLL.Services.UserService;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.BLL.Utils.Email;
using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class UserServiceExtension
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IGenerateJwtToken, GenerateJwtToken>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}

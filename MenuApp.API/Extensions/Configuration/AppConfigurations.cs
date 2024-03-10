using MenuApp.BLL.Configuration;
using MenuApp.DAL.Configurations;

namespace MenuApp.API.Extensions.Configuration
{
    public static class ProjectConfigurationExtension
    {
        public static IServiceCollection ConfigureProjectSettings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}

using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class ConfirmationCodesExtension
    {
        public static IServiceCollection AddConfirmationCodesService(
            this IServiceCollection services
        )
        {
            services.AddScoped<IConfirmationCodesRepository, ConfirmationCodesRepository>();

            return services;
        }
    }
}

using MenuApp.DAL.Repositories;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class ConfirmationCodesExtensions
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

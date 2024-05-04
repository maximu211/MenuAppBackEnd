using MenuApp.BLL.Services.SearchService;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class SearchServiceExtension
    {
        public static IServiceCollection AddSearchService(this IServiceCollection services)
        {
            services.AddScoped<ISearchService, SearchService>();

            return services;
        }
    }
}

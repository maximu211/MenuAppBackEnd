using MenuApp.DAL.Configurations;
using MenuApp.DAL.DataBaseContext;

namespace MenuApp.API.Extensions
{
    public static class MongoDBExtensions
    {
        public static IServiceCollection AddMongoDBConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var mongoDBSettings = configuration.GetSection("MongoDBSettings").Get<DBSettings>();
            services.AddSingleton(
                new DBContext(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName)
            );

            return services;
        }
    }
}

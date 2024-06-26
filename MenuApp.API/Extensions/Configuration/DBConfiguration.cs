﻿using MenuApp.DAL.Configurations;
using MenuApp.DAL.DataBaseContext;

namespace MenuApp.API.Extensions.Configuration
{
    public static class MongoDBExtensions
    {
        public static IServiceCollection AddMongoDBConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var mongoDBSettings = configuration.GetSection("MongoDBSettings").Get<DBSettings>();

            if (mongoDBSettings == null)
            {
                throw new Exception("MongoDB settings are missing or invalid.");
            }

            services.AddSingleton(
                new DBContext(mongoDBSettings.ConnectionString, mongoDBSettings.DatabaseName)
            );

            return services;
        }
    }
}

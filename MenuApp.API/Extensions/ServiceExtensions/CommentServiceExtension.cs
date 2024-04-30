using MenuApp.BLL.Services.CommentSevice;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Extensions.ServiceExtensions
{
    public static class CommentServiceExtension
    {
        public static IServiceCollection AddCommentService(this IServiceCollection services)
        {
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentSevice, CommentService>();

            return services;
        }
    }
}

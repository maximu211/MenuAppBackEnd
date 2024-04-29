using MenuApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MenuApp.BLL.Workers
{
    public class UsersCleanUpWorker : BackgroundService
    {
        private readonly TimeSpan _cleanInterval = TimeSpan.FromHours(1);
        private readonly IServiceProvider _serviceProvider;

        public UsersCleanUpWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var userRepository =
                        scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    await userRepository.DeleteNonConfirmedEmails();
                }

                await Task.Delay(_cleanInterval, stoppingToken);
            }
        }
    }
}

using MenuApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MenuApp.BLL.Workers
{
    public class UsersCleanUpWorker : BackgroundService
    {
        private readonly TimeSpan _cleanInterval = TimeSpan.FromMinutes(1);
        private readonly IServiceProvider _serviceProvider;
        private readonly IUsersRepository _usersRepository;

        public UsersCleanUpWorker(
            IServiceProvider serviceProvider,
            IUsersRepository usersRepository
        )
        {
            _serviceProvider = serviceProvider;
            _usersRepository = usersRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    await _usersRepository.DeleteNonConfirmedEmails();
                }

                await Task.Delay(_cleanInterval, stoppingToken);
            }
        }
    }
}

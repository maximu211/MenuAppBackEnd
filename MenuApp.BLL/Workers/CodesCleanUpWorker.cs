using MenuApp.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MenuApp.BLL.Workers
{
    public class CodesCleanUpWorker : BackgroundService
    {
        private readonly TimeSpan _cleanInterval = TimeSpan.FromMinutes(1);
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfirmationCodesRepository _confirmationCodesRepository;

        public CodesCleanUpWorker(
            IServiceProvider serviceProvider,
            IConfirmationCodesRepository confirmationCodesRepository
        )
        {
            _serviceProvider = serviceProvider;
            _confirmationCodesRepository = confirmationCodesRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    await _confirmationCodesRepository.DeleteExpiredCodes();
                }

                await Task.Delay(_cleanInterval, stoppingToken);
            }
        }
    }
}

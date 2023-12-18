
using Yabi.Models;

namespace Yabi.Cron
{
    public class Scheduler(IHttpClientFactory clientFactory, IServiceScopeFactory _scopeFactory) : BackgroundService
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(2));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await GenerateIndex();
            }
        }

        private async Task GenerateIndex()
        {
            
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<YabiDb>();

            Console.WriteLine("Generating Index....");
            var Yabi = await YabiIndex.Build(clientFactory, db);
            Console.WriteLine($"Index: {Yabi.Index} \n Time: {Yabi.DateTime}");
        }
    }
}

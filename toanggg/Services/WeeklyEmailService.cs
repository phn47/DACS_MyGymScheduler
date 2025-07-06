using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using toanggg.Controllers;
using toanggg.Services;

public class WeeklyEmailService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public WeeklyEmailService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Lấy scope service để resolve các dependency
        using (var scope = _serviceProvider.CreateScope())
        {
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
            await emailService.SendWeeklyEmails();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Không cần làm gì khi dừng
        return Task.CompletedTask;
    }
}

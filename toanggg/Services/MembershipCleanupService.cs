using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;

public class MembershipCleanupService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public MembershipCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Thiết lập thời gian lặp lại (ví dụ: mỗi ngày)
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LinhContext>();

            var expiredMemberships = await context.UserMemberships
                .Where(um => um.EndDate.HasValue && um.EndDate.Value < DateOnly.FromDateTime(DateTime.Today))
                .ToListAsync();

            foreach (var membership in expiredMemberships)
            {
                var user = await context.Users.FindAsync(membership.UserId);
                if (user != null)
                {
                    user.MembershipTypeId = null; // Xóa MembershipTypeId của user
                    context.Update(user);
                }
                context.UserMemberships.Remove(membership);
            }

            await context.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

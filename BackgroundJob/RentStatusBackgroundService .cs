
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.BackgroundJob
{
    public class RentStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public RentStatusBackgroundService(IServiceProvider services)
        {
            _services=services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var property = await context.Property
                            .Where(h => h.Status == "Rented" && h.AvailableDate.Date <= DateTime.Now.Date)
                            .ToListAsync(stoppingToken);

                        foreach (var item in property)
                        {
                            item.Status = "available";
                            context.Property.Update(item);
                        }

                        if (property.Count > 0)
                            await context.SaveChangesAsync(stoppingToken);
                    }

                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Check every day
                                                                           //await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Check every hour
                                                                           //await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Check every day
                }
            }
            catch (Exception ex)
            {
            }
          
        }
    }
}

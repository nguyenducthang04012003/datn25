using Hangfire;
using PharmaDistiPro.Services.Interface;
namespace PharmaDistiPro.Services.Impl
{
    public class MyBackgroundService
    {
        public void ConfigureJobs()
        {
            // Đặt lịch cho công việc chạy mỗi ngày vào lúc 00:00
            RecurringJob.AddOrUpdate<IProductLotService>(
    "auto-update-product-lot-status",
    service => service.AutoUpdateProductLotStatusAsync(),
    "0 0 * * *" // Cron: mỗi ngày lúc 00:00
);
        }
    }
}

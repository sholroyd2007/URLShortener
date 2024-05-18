using Hangfire;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;

namespace URLShortener.Services
{
    public interface IUrlService
    {
        public Task DeleteExpiredUrls();
        void Install();
    }
    public class UrlService : IUrlService
    {
        public UrlService(ApplicationDbContext context)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task DeleteExpiredUrls()
        {
            var links = await Context.Urls.AsNoTracking().Where(e => e.Delete <= DateTime.UtcNow).ToListAsync();
            Context.RemoveRange(links);
            await Context.SaveChangesAsync();
        }

        public void Install()
        {
            RecurringJob.AddOrUpdate("DeleteExpiredUrls", () => DeleteExpiredUrls(), "59 23 * * *");
        }
    }
}

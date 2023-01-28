using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Helpers;

namespace URLShortener.Controllers
{
    [Authorize(Roles = Constants.ADMIN_USER)]
    public class UrlsController : Controller
    {
        public UrlsController(ApplicationDbContext context)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task<IActionResult> Index()
        {
            var urls = await Context.Urls.AsNoTracking().ToListAsync();
            return View(urls);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var url = await Context.Urls.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (url == null)
            {
                return NotFound();
            }
            Context.Remove(url);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Urls");
        }
    }
}

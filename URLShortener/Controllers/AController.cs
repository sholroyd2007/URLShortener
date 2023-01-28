using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.Helpers;

namespace URLShortener.Controllers
{
    public class AController : Controller
    {
        public AController(ApplicationDbContext context)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }


        public async Task<IActionResult> Index(string shortened)
        {
            var url = await Context.Urls.AsNoTracking().FirstOrDefaultAsync(e => e.ShortCode == shortened);
            if(url == null)
            {
                return NotFound();
            }
            url.Clicks += 1;
            Context.Update(url);
            await Context.SaveChangesAsync();
            return Redirect(url.Destination);
        }
       
    }
}

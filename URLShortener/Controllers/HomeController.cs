using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using URLShortener.Data;
using URLShortener.Models;

namespace URLShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static Random rd = new Random();

        public ApplicationDbContext Context { get; }

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            Context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shorten([FromForm] string LongUrl)
        {
            var existing = await Context.Urls.AsNoTracking().FirstOrDefaultAsync(e => e.Destination == LongUrl);
            if (existing == null)
            {
                var newUrl = new Url();
                var newString = GetRandomString(6);
                if (Context.Urls.Any(e => e.ShortCode == newString))
                {
                    return await Shorten(LongUrl);
                }
                newUrl.ShortCode = newString;
                newUrl.Destination = LongUrl;
                Context.Urls.Add(newUrl);
                await Context.SaveChangesAsync();

                return RedirectToAction("Results", new { id = newUrl.Id });
            }
            else
            {
                existing.Delete = DateTime.UtcNow.AddDays(14);
                Context.Urls.Update(existing);
                await Context.SaveChangesAsync();

                return RedirectToAction("Results", new { id = existing.Id });
            }  
        }

        public async Task<IActionResult> Results(int id)
        {
            var url = await Context.Urls.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if(url == null)
            {
                return NotFound();
            }
            return View(url);
        }

        internal static string GetRandomString(int stringLength)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        //Testing Git Changes

    }
}
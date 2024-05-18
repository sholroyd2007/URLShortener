using URLShortener.Data;
using URLShortener.Helpers;
using URLShortener.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using URLShortener.Services;

namespace URLShortener.Controllers
{

    public class InstallController : Controller
    {
        public InstallController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IUrlService urlService)
        {
            RoleManager = roleManager;
            UserManager = userManager;
            Context = context;
            UrlService = urlService;
        }

        public RoleManager<IdentityRole> RoleManager { get; }
        public UserManager<IdentityUser> UserManager { get; }
        public ApplicationDbContext Context { get; }
        public IUrlService UrlService { get; }

        public async Task<IActionResult> Index()
        {
            var exists = await RoleManager.RoleExistsAsync(Constants.ADMIN_USER);
            if (!exists)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin@shortener.app",
                    Email = "admin@shortener.app",
                };
                await UserManager.CreateAsync(adminUser, "P@ssw0rd!23");
                await RoleManager.CreateAsync(new IdentityRole { Name = Constants.ADMIN_USER });
                await UserManager.AddToRoleAsync(adminUser, Constants.ADMIN_USER);

                UrlService.Install();

                await Context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}

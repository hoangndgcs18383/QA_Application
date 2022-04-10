using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QA_Application.Data;
using QA_Application.Models;

namespace QA_Application.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manager/[action]")]
    public class DbManageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DbManageController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _context.Database.EnsureCreatedAsync();

            StatusMessage = success ? "Delete Database suceess" : "Unable delete Database";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migration()
        {
            await _context.Database.MigrateAsync();

            StatusMessage = "Update Database suceess";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeedDataAsync()
        {
            var roleAdmin = await _roleManager.FindByIdAsync("Admin");
            if (roleAdmin == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName: "Admin"));
            }

            var userAdmin = await _userManager.FindByEmailAsync("admin");
            if(userAdmin == null)
            {
                userAdmin = new ApplicationUser()
                {
                    UserName = "admin",
                    FirstName = "admin",
                    LastName = " ",
                    Email = "admin@fpt.edu.vn",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(userAdmin, "admin123@");
                await _userManager.AddToRoleAsync(userAdmin, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

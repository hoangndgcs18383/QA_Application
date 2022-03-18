using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace QA_Application.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["Created"] = "Role has been created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

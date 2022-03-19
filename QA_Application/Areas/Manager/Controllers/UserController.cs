using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QA_Application.Data;
using QA_Application.Models;

namespace QA_Application.Areas.Account.Controllers
{
    [Area("Manager")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;

        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.ApplicationUsers.ToList());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var identity = await _userManager.CreateAsync(user, user.PasswordHash);
                if (identity.Succeeded)
                {
                    var isSaveRole = await _userManager.AddToRoleAsync(user, role: "User");
                    TempData["Created"] = "Account has been created successfully";
                    return RedirectToAction("Index");
                }
                foreach(var error in identity.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return BadRequest();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = _context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if(userInfo == null) return BadRequest();

            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            var identity = await _userManager.UpdateAsync(userInfo);
            if (identity.Succeeded)
            {
                TempData["Edited"] = "Account has been edited successfully";
                return RedirectToAction("Index");
            }


            return View();
        }

        public async Task<IActionResult> Lockout(string id)
        {
            if(id == null) return BadRequest();

            var user = _context.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if(user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Lockout(ApplicationUser user)
        {
            var userInfo = _context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null) return NotFound();

            userInfo.LockoutEnd = DateTime.Now.AddYears(100);
            int rowAffectd = await _context.SaveChangesAsync();

            if(rowAffectd > 0)
            {
                TempData["Deleted"] = "Account has been lockout successfully";
                return RedirectToAction("Index");
            }

            return View(userInfo);
        }

        public async Task<IActionResult> Active(string id)
        {
            if (id == null) return BadRequest();

            var user = _context.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userInfo = _context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null) return NotFound();

            userInfo.LockoutEnd = DateTime.Now.AddYears(-1);
            int rowAffectd = await _context.SaveChangesAsync();

            if (rowAffectd > 0)
            {
                TempData["Deleted"] = "Account has been actived successfully";
                return RedirectToAction("Index");
            }

            return View(userInfo);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return BadRequest();

            var user = _context.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userInfo = _context.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null) return NotFound();

            _context.ApplicationUsers.Remove(userInfo);
            int rowAffectd = await _context.SaveChangesAsync();

            if (rowAffectd > 0)
            {
                TempData["Deleted"] = "Account has been deleted successfully";
                return RedirectToAction("Index");
            }

            return View(userInfo);
        }
    }
}

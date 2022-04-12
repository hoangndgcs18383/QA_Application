using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QA_Application.Areas.Manager.ViewModels;
using QA_Application.Data;
using QA_Application.Models;
using System.ComponentModel;

namespace QA_Application.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _context;
        
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            ViewBag.cRoles = roles.Count;
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

            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.message = "This role is already exit";
                ViewBag.name = name;
                return View();
            }

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "Role has been created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest();

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id ,string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest();

            role.Name = name;

            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.message = "This role is already exit";
                ViewBag.name = name;
                return View();
            }

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "Role has been updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return BadRequest();

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null) return BadRequest();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Role has been deleted successfully";
                return RedirectToAction("Index");
            }

            return View();
        }
        public async Task<IActionResult> Assign()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers
                .Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null)
                .ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleViewModel userRoleViewModel)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(c => c.Id == userRoleViewModel.UserId);
            var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, userRoleViewModel.RoleId);


            if (isCheckRoleAssign)
            {
                ViewData["UserId"] = new SelectList(_context.ApplicationUsers
                    .Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null)
                    .ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                ViewBag.message = "This role already assign this role.";
                return View();
            }

            var addRoleToUser = await _userManager.AddToRoleAsync(user, userRoleViewModel.RoleId);
            if (addRoleToUser.Succeeded)
            {
                TempData["Success"] = "Role has been assigned successfully";
                return RedirectToAction("Index");
            }
            return View();
        }


        public async Task<IActionResult> AssignUserRole()
        {
            var result = from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.Id
                         join a in _context.ApplicationUsers on ur.RoleId equals a.Id
                         select new UserRoleMaping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };

            ViewBag.UserRoles = result;
            return View();
        }
    }
}

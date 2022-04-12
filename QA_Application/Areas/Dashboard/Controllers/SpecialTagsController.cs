#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QA_Application.Data;
using QA_Application.Models;

namespace QA_Application.Areas.Admin.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = ("Admin, Manager"))]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SpecialTags
        public async Task<IActionResult> Index()
        {
            var tags = await _context.SpecialTags.ToListAsync();

            ViewBag.Tags = tags.Count;

            return View(tags);
        }


        // GET: Admin/SpecialTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SpecialTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecialTagName")] SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                TempData["Success"] = "Special Tag has been created successfully";
                _context.Add(specialTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }

        // GET: Admin/SpecialTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        // POST: Admin/SpecialTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpecialTagName")] SpecialTag specialTag)
        {
            if (id != specialTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TempData["Success"] = "Special Tag has been edited successfully";
                    _context.Update(specialTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialTagExists(specialTag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }

        // GET: Admin/SpecialTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        // POST: Admin/SpecialTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["Success"] = "Category has been deleted successfully";
            var specialTag = await _context.SpecialTags.FindAsync(id);
            _context.SpecialTags.Remove(specialTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialTagExists(int id)
        {
            return _context.SpecialTags.Any(e => e.Id == id);
        }
    }
}

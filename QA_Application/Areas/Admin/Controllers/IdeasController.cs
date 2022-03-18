#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QA_Application.Data;
using QA_Application.Models;

namespace QA_Application.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IdeasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Ideas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ideas.Include(i => i.Category).Include(i => i.SpecialTag);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Ideas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // GET: Admin/Ideas/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "Name");
            return View();
        }

        // POST: Admin/Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedDate,isAvailable,CategoryId,SpecialTagId")] Idea idea)
        {
            if (!ModelState.IsValid)
            {
                TempData["Created"] = "Ideas has been created successfully";
                idea.CreatedDate = DateTime.Now;
                _context.Add(idea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "Name", idea.SpecialTagId);
            return View(idea);
        }

        // GET: Admin/Ideas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "Name", idea.SpecialTagId);
            return View(idea);
        }

        // POST: Admin/Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedDate,isAvailable,CategoryId,SpecialTagId")] Idea idea)
        {
            if (id != idea.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    TempData["Edited"] = "Ideas has been edited successfully";
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdeaExists(idea.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "Name", idea.SpecialTagId);
            return View(idea);
        }

        // GET: Admin/Ideas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // POST: Admin/Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["Deleted"] = "Ideas has been deleted successfully";
            var idea = await _context.Ideas.FindAsync(id);
            _context.Ideas.Remove(idea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeaExists(int id)
        {
            return _context.Ideas.Any(e => e.Id == id);
        }
    }
}

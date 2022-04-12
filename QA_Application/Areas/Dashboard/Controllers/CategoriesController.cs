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

namespace QA_Application.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = ("Admin, Manager"))]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Categories1
        public async Task<IActionResult> Index()
        {
            var qr = (from c in _context.Categories select c)
                .Include(c => c.ParentCategory)
                .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            ViewBag.Categories = categories.Count;

            return View(categories);
        }

        // GET: Dashboard/Categories1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Idea)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Dashboard/Categories1/Create
        public async Task<IActionResult> CreateAsync()
        {
            var qr = (from c in _context.Categories select c)
                            .Include(c => c.ParentCategory)
                            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            categories.Insert(0 ,new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            var items = new List<Category>();
            CreateSelectItem(categories, items, 0);

            var selectLists = new SelectList(items, "Id", "CategoryName");

            ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content");
            ViewData["ParentCategoryId"] = selectLists;
            return View();
        }

        // POST: Dashboard/Categories1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Description,FirstDeadline,FinalDeadline,ParentCategoryId")] Category category)
        {
            var qr = (from c in _context.Categories select c)
                .Include(c => c.ParentCategory)
                .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            categories.Add( new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            categories.Insert(0, new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            var items = new List<Category>();
            CreateSelectItem(categories, items, 0);

            var selectLists = new SelectList(items, "Id", "CategoryName");

            if (!ModelState.IsValid)
            {
                var search = _context.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (search != null)
                {
                    ViewBag.error = "The Category Name has exits";
                    ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content", category.IdeaId);
                    ViewData["ParentCategoryId"] = selectLists;
                    return View(category);
                }

                if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                /*if(category.FirstDeadline < category.ParentCategory.FirstDeadline)
                {
                    ViewBag.error = "First deadline should more than Parent";
                    return View(category);
                }*/
                TempData["Success"] = "Category has been created successfully";
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content", category.IdeaId);
            ViewData["ParentCategoryId"] = selectLists;
            return View(category);
        }

        // GET: Dashboard/Categories1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

                        var qr = (from c in _context.Categories select c)
                            .Include(c => c.ParentCategory)
                            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            categories.Insert(0, new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            var items = new List<Category>();
            CreateSelectItem(categories, items, 0);

            var selectLists = new SelectList(items, "Id", "CategoryName");

            ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content", category.IdeaId);
            ViewData["ParentCategoryId"] = selectLists;

            return View(category);
        }

        // POST: Dashboard/Categories1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName,Description,FirstDeadline,FinalDeadline,ParentCategoryId,IdeaId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if(category.ParentCategoryId == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Error!! You need to select other parent category");
            }

            if(category.FirstDeadline < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Error!! You need to choose date greater than now");
            }

            if (!ModelState.IsValid && category.ParentCategoryId != category.Id)
            {
                try
                {
                    if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                    TempData["Success"] = "Special Tag has been edited successfully";
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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

            var qr = (from c in _context.Categories select c)
                            .Include(c => c.ParentCategory)
                            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            categories.Insert(0, new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            var items = new List<Category>();
            CreateSelectItem(categories, items, 0);

            var selectLists = new SelectList(items, "Id", "CategoryName");

            ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content", category.IdeaId);
            ViewData["ParentCategoryId"] = selectLists;
            return View(category);
        }

        // GET: Dashboard/Categories1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Idea)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            var qr = (from c in _context.Categories select c)
                            .Include(c => c.ParentCategory)
                            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            categories.Insert(0, new Category()
            {
                Id = -1,
                CategoryName = "No title"
            });

            var items = new List<Category>();
            CreateSelectItem(categories, items, 0);

            var selectLists = new SelectList(items, "Id", "CategoryName");

            ViewData["IdeaId"] = new SelectList(_context.Ideas, "Id", "Content");
            ViewData["ParentCategoryId"] = selectLists;

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Dashboard/Categories1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.CategoryChildren)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);

            if(category == null)
            {
                return BadRequest();
            }

            foreach(var cCat in category.CategoryChildren)
            {
                cCat.ParentCategoryId = category.ParentCategoryId;
            }
            TempData["Success"] = "Special Tag has been deleted successfully";
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private void CreateSelectItem(List<Category> source, List<Category> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("----", level));
            foreach (var category in source)
            {
                //category.CategoryName = prefix + " " + category.CategoryName;
                des.Add(new Category() { 
                    Id = category.Id,
                    CategoryName = prefix + " " + category.CategoryName
                });;
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItem(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }
    }
}

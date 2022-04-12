using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PagedList;
using QA_Application.Data;
using QA_Application.Models;
using QA_Application.ViewModels;
using static QA_Application.Helper;

namespace QA_Application.Areas.Admin.Controllers
{
    [Area("Dashboard")]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IdeasController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Admin/Ideas
        [Authorize]
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ideas = _context.Ideas.
                Include(i => i.Author).
                Include(i => i.Category).
                Include(i => i.SpecialTag).
                Include(i => i.Department).
                Where(c => c.isApproved == null || c.isApproved == false && c.AuthorId == userId);


            int totalIdeas = await ideas.CountAsync();
            if (pageSize <= 0) pageSize = 5;
            int countPages = (int)Math.Ceiling((double)totalIdeas / pageSize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagerModel = new Pager()
            {
                countPages = countPages,
                currentPage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pageSize = pageSize
                })
            };

            ViewBag.pagerModel = pagerModel;
            ViewBag.totalIdeas = totalIdeas;

            ViewBag.ideaIndex = (currentPage - 1) * pageSize;

            var ideasInPage = await ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Department)
                .OrderByDescending(o => o.CreatedDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(ideasInPage);
        }

        [Authorize]
        // GET: Admin/GetPost
        public async Task<IActionResult> GetPost(Idea idea, [FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ideas = _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Department)
                .Where(c => c.isApproved == true);

            var ideasById = _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Department)
                .Where(c => c.AuthorId == userId && c.isApproved != true);

            int totalApproved = await ideasById.CountAsync();

            int totalIdeas = await ideas.CountAsync();

            if (pageSize <= 0) pageSize = 5;
            int countPages = (int)Math.Ceiling((double)totalIdeas / pageSize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagerModel = new Pager()
            {
                countPages = countPages,
                currentPage = currentPage,
                generateUrl = (pageNumber) => Url.Action("GetPost", new
                {
                    p = pageNumber,
                    pageSize = pageSize
                })
            };

            ViewBag.pagerModel = pagerModel;
            ViewBag.totalIdeas = totalIdeas;
            ViewBag.ideaIndex = (currentPage - 1) * pageSize;
            ViewBag.ideasNotAp = totalApproved;



            var ideasInPage = await ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(i => i.CountThumb)
                .ToListAsync();

            return View(ideasInPage);
        }

        // GET: Admin/Ideas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Comments)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.count = idea.CountThumb;
            ViewBag.like = idea.CountThumbUp;
            ViewBag.dislike = idea.CountThumbDown;

            var dateTime = Time(idea.CreatedDate);

            ViewBag.time = dateTime;

            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // GET: Admin/Ideas/Details/5
        public async Task<IActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Comments)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);


            var dateTime = Time(idea.CreatedDate);

            ViewBag.time = dateTime;

            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.SpecialTag)
                .Include(i => i.Department)
                .Include(i => i.Comments)
                    .ThenInclude(i => i.Author)
                .FirstOrDefaultAsync(i => i.Id == model.IdeaId);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (model.CommentId == 0)
            {
                idea.Comments = idea.Comments ?? new List<Comment>();

                idea.Comments.Add(new Comment
                {
                    Message = model.Message,
                    CreatedDate = DateTime.Now,
                    AuthorId = userId,
                });
                _context.Update(idea);

            }


            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = model.IdeaId });


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveComment(int id)
        {

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ThumbUp(int id, ThumbViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            TempData["Success"] = "Ideas has been deleted successfully";
            /*var idea = await _context.Ideas.FindAsync(id);*/
            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.Department)
                .Include(i => i.SpecialTag)
                .Include(i => i.Comments)
                    .ThenInclude(i => i.Author)
                .Include(i => i.Thumbs)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (model.ThumbId == 0)
            {
                var search = idea.Thumbs.FirstOrDefault(i => i.AuthorId == userId && i.toggle == "like" && i.isThumb == true);

                if (search != null)
                {
                    _context.Remove(search);
                    idea.CountThumb--;
                    idea.CountThumbUp--;
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }

                if (search == null)
                {
                    idea.Thumbs = idea.Thumbs ?? new List<Thumb>();

                    idea.Thumbs.Add(new Thumb
                    {
                        AuthorId = userId,
                        toggle = "like",
                        isThumb = true
                    });

                    idea.CountThumb++;
                    idea.CountThumbUp++;
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    return RedirectToAction("Details", new { id = id });
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ThumbDown(int id, ThumbViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            TempData["Success"] = "Ideas has been deleted successfully";
            /*var idea = await _context.Ideas.FindAsync(id);*/
            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.Department)
                .Include(i => i.SpecialTag)
                .Include(i => i.Comments)
                    .ThenInclude(i => i.Author)
                .Include(i => i.Thumbs)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (model.ThumbId == 0)
            {
                var search = idea.Thumbs.FirstOrDefault(i => i.AuthorId == userId && i.toggle == "dislike" && i.isThumb == true);

                if (search != null)
                {
                    _context.Remove(search);
                    idea.CountThumb++;
                    idea.CountThumbDown--;
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }

                if (search == null)
                {
                    idea.Thumbs = idea.Thumbs ?? new List<Thumb>();

                    idea.Thumbs.Add(new Thumb
                    {
                        AuthorId = userId,
                        toggle = "dislike",
                        isThumb = true,
                    });

                    idea.CountThumb--;
                    idea.CountThumbDown++;
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    return RedirectToAction("Details", new { id = id });
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });

        }

        // GET: Admin/Ideas/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName");
            ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName");
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName");
            return View();
        }

        // POST: Admin/Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedDate,CategoryId,SpecialTagId,AuthorId,DepartmentId")] Idea idea, IFormFile fileSubmit)
        {

            if (idea.Title.Length < 4)
            {
                ModelState.AddModelError(string.Empty, "Error!! You need to enter more charater");
            }

            if (!ModelState.IsValid)
            {
                var search = _context.Ideas.FirstOrDefault(c => c.Title == idea.Title);
                if (search != null)
                {
                    ViewBag.error = "This title is already exist";
                    ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
                    ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName", idea.AuthorId);
                    ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
                    ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
                    return View(idea);
                }

                if (fileSubmit != null)
                {
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }


                    string fullPath = Path.Combine(path, Path.GetFileName(fileSubmit.FileName));
                    using (FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await fileSubmit.CopyToAsync(file);
                    }

                    idea.FileSubmit = Path.GetFileName(fileSubmit.FileName);
                }

                if (fileSubmit == null)
                {
                    idea.FileSubmit = "nothing.pdf";
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                idea.AuthorId = userId;
                idea.isLocked = false;
                _context.Add(idea);
                TempData["Success"] = "Ideas has been created successfully";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
            ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName", idea.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
            return View(idea);
        }

        // GET: Admin/Ideas/Edit/5
        [Authorize]
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

            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName");
            ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName");
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName");
            return View(idea);
        }

        // POST: Admin/Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedDate,CategoryId,SpecialTagId,AuthorId,DepartmentId")] Idea idea, IFormFile fileSubmit)
        {
            if (id != idea.Id)
            {
                return NotFound();
            }

            if (idea.Title.Length < 1)
            {
                ModelState.AddModelError(string.Empty, "Error!! You need to enter more charater");
            }

            if (!ModelState.IsValid)
            {
                /*                var search = _context.Ideas.FirstOrDefault(c => c.Title == idea.Title);
                                if (search != null)
                                {
                                    ViewBag.error = "This title is already exist";
                                    ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
                                    ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName", idea.AuthorId);
                                    ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
                                    ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
                                    return View(idea);
                                }*/

                try
                {

                    if (fileSubmit != null)
                    {
                        string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        /*string folder = "Uploads";
                        idea.FileSubmit = await UploadImage(folder, myFile);*/

                        string fullPath = Path.Combine(path, Path.GetFileName(fileSubmit.FileName));
                        using (FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                        {
                            await fileSubmit.CopyToAsync(file);
                        }

                        idea.FileSubmit = "Uploads/" + Path.GetFileName(fileSubmit.FileName);
                    }

                    if (fileSubmit == null)
                    {
                        idea.FileSubmit = "Uploads/nothing.pdf";
                    }

                    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    idea.isApproved = null;
                    idea.AuthorId = userId;
                    idea.isLocked = false;
                    idea.LastUpdateDate = DateTime.Now;
                    TempData["Success"] = "Ideas has been edited successfully";
                    _context.Update(idea);
                    _context.Update(idea).Property("CreatedDate").IsModified = false;
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

            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
            ViewData["AuthorId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName", idea.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
            return View(idea);
        }

        // POST: Admin/Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["Success"] = "Ideas has been deleted successfully";
            /*var idea = await _context.Ideas.FindAsync(id);*/
            var idea = await _context.Ideas
                .Include(i => i.Author)
                .Include(i => i.Category)
                .Include(i => i.Department)
                .Include(i => i.SpecialTag)
                .Include(i => i.Comments)
                    .ThenInclude(i => i.Author)
                .Include(i => i.Thumbs)
                    .ThenInclude(i => i.Author)
                .FirstOrDefaultAsync(i => i.Id == id);


            _context.Ideas.Remove(idea).Entity.Comments.RemoveAll(c => c.Message == null);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeaExists(int id)
        {
            return _context.Ideas.Any(e => e.Id == id);
        }


        // POST: Admin/Ideas/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Approve(Idea idea)
        {
            if (!ModelState.IsValid)
            {
                await GetIdeaByAction(idea, "approve", true);
                return RedirectToAction(nameof(Index));

            }

            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
            return View(idea);
        }

        // POST: Admin/Ideas/Decline
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Decline([Bind("Id,Title,Content,CreatedDate,CategoryId,SpecialTagId,AuthorId,DepartmentId")] Idea idea)
        {
            if (!ModelState.IsValid)
            {
                await GetIdeaByAction(idea, "decline", false);
                return RedirectToAction(nameof(Index));

            }

            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
            return View(idea);
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }

        // POST: Admin/Ideas/Locked
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Locked([Bind("Id,Title,Content,CreatedDate,CategoryId,SpecialTagId,AuthorId,DepartmentId")] Idea idea)
        {
            if (!ModelState.IsValid)
            {
                await GetIdeaByAction(idea, "lock", false);
                return RedirectToAction(nameof(Index));

            }

            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmentName", idea.DepartmentId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.FinalDeadline > DateTime.Now), "Id", "CategoryName", idea.CategoryId);
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "Id", "SpecialTagName", idea.SpecialTagId);
            return View(idea);
        }


        //Repository

        private async Task<Idea> GetIdeaByAction(Idea idea, string action, bool isApprove)
        {
            TempData["Success"] = "Ideas has been " + action + " successfully";
            _context.Update(idea).Entity.isApproved = isApprove;
            _context.Entry(idea).Property("Id").IsModified = false;
            _context.Entry(idea).Property("Content").IsModified = false;
            _context.Entry(idea).Property("Title").IsModified = false;
            _context.Entry(idea).Property("AuthorId").IsModified = false;
            _context.Entry(idea).Property("CreatedDate").IsModified = false;
            _context.Entry(idea).Property("LastUpdateDate").IsModified = false;
            _context.Entry(idea).Property("FileSubmit").IsModified = false;
            _context.Entry(idea).Property("CategoryId").IsModified = false;
            _context.Entry(idea).Property("SpecialTagId").IsModified = false;
            _context.Entry(idea).Property("DepartmentId").IsModified = false;
            await _context.SaveChangesAsync();
            return idea;
        }

        private static string Time(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
    }

}

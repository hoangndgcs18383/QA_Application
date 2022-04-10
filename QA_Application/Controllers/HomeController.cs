using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QA_Application.Data;
using QA_Application.Models;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;


namespace QA_Application.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        

        public IActionResult Index()
        {
            return View();
        }

/*        public IActionResult GetData()
        {
            List<int> repartitions = new List<int>();
            var ideaPerDepartment = _context.Ideas.Select(x => x.Department).Distinct();

            foreach(var item in ideaPerDepartment)
            {
                repartitions.Add()
            }

            

            return Json();
        }*/

        public class Ratio
        {
            public int Idea { get; set; }
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
    }
}
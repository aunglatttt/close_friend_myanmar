using System.Diagnostics;
using System.Security.Claims;
using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context=context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.Include(x => x.ClientRequirements).ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SuccessComponent(string Title, string Message, string ActionName, string ActionName2, string BtnName, string ControllerName)
        {
            ViewBag.TitleName = Title;
            ViewBag.Message = Message;
            ViewBag.Action = ActionName;
            ViewBag.Action2 = ActionName2;
            ViewBag.BtnName = BtnName;
            ViewBag.Controller = ControllerName;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

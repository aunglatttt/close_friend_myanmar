using System.Diagnostics;
using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloseFriendMyanamr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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

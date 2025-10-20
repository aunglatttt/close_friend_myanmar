using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
            int count = await _context.Alert.AsNoTracking().Where(x => x.Status != "Read").CountAsync();
            var alert = await _context.Alert.AsNoTracking().OrderByDescending(x => x.Id).Take(3).Where(x => x.Status != "Read").ToListAsync();

            var code = alert.Select(x => x.Code).ToList();

            var propertyByCode = await _context.Property.AsNoTracking()
                .Select(x => new
                {
                    x.Code,
                    x.Id,
                    x.AvailableDate
                })
                .Where(x => code.Contains(x.Code??""))
                .ToListAsync();

            var alertModels = new List<AlertViewModel>();
            foreach (var item in alert)
            {
                var property = propertyByCode.FirstOrDefault(x => x.Code == item.Code);

                alertModels.Add(new AlertViewModel
                {
                    Id = item.Id,
                    PropertyId = property == null ? 0 : property.Id,
                    Message = item.Message,
                    AvailableDate = property == null ? null : property.AvailableDate,
                    Code = item.Code,
                });
            }

            ViewBag.Notifications = alertModels;
            ViewBag.NotiCount = count;

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

        public IActionResult Calculator()
        {
            return View();
        }

        public async Task<IActionResult> ShowCodeNumberDetail()
        {
            try
            {
                var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
                if (cpiClaim == null)
                    return RedirectToAction("Login", "Account");

                if (!int.TryParse(cpiClaim.Value, out int cpi))
                {
                    ViewBag.Error = "Invalid CPI value";
                    return View("Error");
                }

                string pattern = @"([A-Za-z]+)(\d+)";

                var codes = await _context.Property.AsNoTracking()
                    .Where(x => x.CPI == cpi)
                    .Select(x => x.Code)
                    .ToListAsync();

                var maxCodes = codes
                   .Select(code =>
                   {
                       var match = Regex.Match(code, pattern);
                       if (match.Success)
                       {
                           return new
                           {
                               Prefix = match.Groups[1].Value, 
                               Number = int.Parse(match.Groups[2].Value)
                           };
                       }
                       return null;
                   })
                   .Where(x => x != null) 
                   .GroupBy(x => x.Prefix)
                   .Select(group => new CodeListViewModel
                   {
                       Prefix = group.Key,
                       MaxCode = group.Max(x => x.Number)
                   })
                   .ToList();

                var propertyTypes = await _context.PropertyType.AsNoTracking().Select(x => new { x.TypeName, x.ShortCode }).ToListAsync();
                foreach (var item in maxCodes)
                {
                    item.PropertyType = propertyTypes.Where(x => x.ShortCode == item.Prefix).Select(x => x.TypeName).FirstOrDefault()??"";
                }

                return View(maxCodes.OrderBy(x => x.PropertyType).ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

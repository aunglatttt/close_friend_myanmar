using Microsoft.AspNetCore.Mvc;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyController(ApplicationDbContext context)
        {
            _context=context;
        }

        public IActionResult NewProperty()
        {
            return View();
        }
    }
}

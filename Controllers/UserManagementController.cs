using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserManagementController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> ClientList()
        {
            return View(await _context.Client.AsNoTracking().ToListAsync());
        }


        #region Owner
        public async Task<IActionResult> OwnerList()
        {
            return View(await _context.Owner.AsNoTracking().ToListAsync());
        }

        #endregion
    }
}

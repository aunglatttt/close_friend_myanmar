using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context=context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region employee type 
        public async Task<IActionResult> EmployeeTypeList()
        {
            return View(await _context.EmployeeTypes.ToListAsync());
        }

        public async Task<IActionResult> DetailEmployeeType(int id)
        {
            var epmtype = await _context.EmployeeTypes.FindAsync(id);
            return Json(epmtype);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeType(EmployeeType expense)
        {
            expense.CreatedAt = DateTime.Now;
            _context.EmployeeTypes.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeTypeList));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeType(EmployeeType expense)
        {
            expense.UpdatedAt = DateTime.Now;
            _context.EmployeeTypes.Update(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeTypeList));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployeeType(int id)
        {
            var expense = await _context.EmployeeTypes.FindAsync(id);
            if (expense != null)
            {
                _context.EmployeeTypes.Remove(expense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EmployeeTypeList));
        }
        #endregion
    }
}

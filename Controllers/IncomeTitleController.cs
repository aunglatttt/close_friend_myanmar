using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class IncomeTitleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeTitleController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.IncomeTitles.ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> Add(IncomeTitleModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required");

            item.CreatedAt = DateTime.Now;
            _context.IncomeTitles.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(IncomeTitleModel item)
        {
            var existingItem = await _context.IncomeTitles.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.IncomeTitles.FindAsync(id);
            if (item == null) return NotFound();

            _context.IncomeTitles.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var epmtype = await _context.IncomeTitles.FindAsync(id);
            return Json(epmtype);
        }
    }
}

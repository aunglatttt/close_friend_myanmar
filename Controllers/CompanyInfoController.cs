using CloseFriendMyanamr.Models.CompanyInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;
using System.Threading.Tasks;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class CompanyInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }
            var model = await _context.CompanyInformation.FirstOrDefaultAsync(x => x.CPI == cpi);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Fetch the existing company data from your database/service
            var companyInfo = await _context.CompanyInformation.FirstOrDefaultAsync(x => x.CPI == id.Value);

            if (companyInfo == null)
            {
                return NotFound();
            }

            // 2. Pass the model to the view
            return View(companyInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int CPI, CompanyInfoModel model)
        {
            if (CPI != model.CPI)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.UpdatedAt = DateTime.Now;   
                    _context.CompanyInformation.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) // Good practice for concurrency
                {
                    // Check if the record truly exists before throwing an error
                    if (!await _context.CompanyInformation.AnyAsync(x => x.CPI == model.CPI))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // 3. Redirect to the main view after successful update
                return RedirectToAction(nameof(Index)); // Or whatever your main view is called
            }

            // If validation fails, return the model back to the view
            return View(model);
        }
    }
}

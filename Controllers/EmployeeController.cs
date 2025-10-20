using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            #region get cpi
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }
            #endregion

            return View(await _context.Employee.AsNoTracking().Include(d => d.EmployeeType).Where(x => x.CPI == cpi).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            #region get cpi
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }
            #endregion

            var empTypes = await _context.EmployeeType.AsNoTracking()
                .Where(x => x.CPI == cpi && !x.IsDeleted)
                .Select(x => new { x.Id, x.Type })
                .ToListAsync();

            if (empTypes == null || !empTypes.Any())
            {
                // Handle the case where no Employee Types are found (optional)
                ViewData["EmployeeType"] = new SelectList(new List<object>(), "Id", "Type");
            }
            else
            {
                ViewData["EmployeeType"] = new SelectList(empTypes, "Id", "Type");
            }

            ViewData["Title"] = "Create Employee";

            if (id != null && id > 0)
            {
                return View(await _context.Employee.FindAsync(id));
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(EmployeeModel model)
        {
            #region get cpi
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }
            #endregion

            if (ModelState.IsValid)
            {

                string returnMsg = "Created";
                if (model.Id > 0)
                {
                    var existingModel = await _context.Employee.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.CPI = cpi;
                        existingModel.EmployeeName = model.EmployeeName;
                        existingModel.PhoneNumber = model.PhoneNumber;
                        existingModel.EmployeeTypeId = model.EmployeeTypeId;
                        existingModel.LoginName = model.LoginName;
                        existingModel.Password = model.Password;
                        existingModel.Status = model.Status;
                        model.UpdatedAt = DateTime.Now;

                        returnMsg = "Updated";
                    }
                }
                else
                {
                    model.CPI = cpi;
                    model.CreatedAt = DateTime.Now;
                    _context.Employee.Add(model);

                    returnMsg = "Created";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Success", new { message = returnMsg });
            }

            var empTypes = await _context.EmployeeType.AsNoTracking()
                                .Where(x => x.CPI == cpi && !x.IsDeleted)

                .Select(x => new { x.Id, x.Type }) // Ensure x.Type is used
                .ToListAsync();

            if (empTypes == null || !empTypes.Any())
            {
                // Handle the case where no Employee Types are found (optional)
                ViewData["EmployeeType"] = new SelectList(new List<object>(), "Id", "Type");
            }
            else
            {
                ViewData["EmployeeType"] = new SelectList(empTypes, "Id", "Type");
            }
            // If the model is invalid, re-display the form with validation errors.
            return View(model);
        }

        public IActionResult Success(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        #region employee type 
        public async Task<IActionResult> EmployeeTypeList()
        {
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }

            ViewData["FormTitle"] = "Add Employee Type";
            return View(await _context.EmployeeType.AsNoTracking().Where(x => x.CPI == cpi && !x.IsDeleted).ToListAsync());
        }

        public async Task<IActionResult> DetailEmployeeType(int id)
        {
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }

            var epmtype = await _context.EmployeeType.Where(x => x.CPI == cpi && x.Id == id).FirstOrDefaultAsync();
            return Json(epmtype);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeeType(EmployeeType expense)
        {
            var cpiClaim = User.Claims.FirstOrDefault(c => c.Type == "CPI");
            if (cpiClaim == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(cpiClaim.Value, out int cpi))
            {
                ViewBag.Error = "Invalid CPI value";
                return View("Error");
            }

            expense.CreatedAt = DateTime.Now;
            expense.CPI = cpi;
            _context.EmployeeType.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeTypeList));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeType(EmployeeType expense)
        {
            expense.UpdatedAt = DateTime.Now;
            _context.EmployeeType.Update(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeeTypeList));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployeeType(int id)
        {
            var expense = await _context.EmployeeType.FindAsync(id);
            if (expense != null)
            {
                //_context.EmployeeType.Remove(expense);
                expense.IsDeleted = true;
                _context.EmployeeType.Update(expense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EmployeeTypeList));
        }
        #endregion
    }
}

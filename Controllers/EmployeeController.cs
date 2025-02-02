using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.AsNoTracking().Include(d => d.EmployeeType).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            // Fetch employee types from the database
            var empTypes = await _context.EmployeeTypes.AsNoTracking()
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

            ViewData["Title"] = "Create Employee";

            if(id != null && id > 0)
            {
                return View(await _context.Employees.FindAsync(id));
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(EmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                // Insert your data saving logic here (e.g., save to the database)
                if (model.Id > 0)
                {
                    var existingModel = await _context.Employees.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.EmployeeName = model.EmployeeName;
                        existingModel.PhoneNumber = model.PhoneNumber;
                        existingModel.EmployeeTypeId =  model.EmployeeTypeId;
                        existingModel.LoginName = model.LoginName;
                        existingModel.Password = model.Password;
                        existingModel.Status = model.Status;
                        model.UpdatedAt = DateTime.Now;
                    }
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    _context.Employees.Add(model);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Success", new { message = model.Id > 0? "Updated" : "Created" });
            }

            var empTypes = await _context.EmployeeTypes.AsNoTracking()
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
            ViewData["FormTitle"] = "Add Employee Type";
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

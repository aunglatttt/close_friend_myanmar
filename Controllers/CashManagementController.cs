using CloseFriendMyanamr.Models.CashManagement;
using CloseFriendMyanamr.Models.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;
using System;

namespace CloseFriendMyanamr.Controllers
{
    public class CashManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashManagementController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> CompanyIncome(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            var incomeTitle = await _context.IncomeTitle.AsNoTracking()
           .Select(x => new { x.Id, x.Name })
           .ToListAsync();

            if (incomeTitle == null || !incomeTitle.Any())
            {
                ViewData["IncomeTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["IncomeTitle"] = new SelectList(incomeTitle, "Id", "Name");
            }
            if (id != null && id > 0)
            {
                return View(await _context.Income.FindAsync(id));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompanyIncome(CompanyIncome model)
        {
            if (ModelState.IsValid)
            {
                string title = "Company Income Created";
                //string incomeTileName = (model.IncomeTitleId != null && model.IncomeTitleId > 0) ?
                //    await _context.IncomeTitle.AsNoTracking().Where(x => x.Id == model.IncomeTitleId).Select(x => x.Name).FirstOrDefaultAsync()??""
                //    : "";

                //if (model.Id > 0)
                //{
                //    var existingModel = await _context.Income.FindAsync(model.Id);
                //    if (existingModel != null)
                //    {
                //        existingModel.IncomeDate = model.IncomeDate;
                //        existingModel.IncomeTitleId = model.IncomeTitleId;
                //        existingModel.IncomeTitleName =  incomeTileName;
                //        existingModel.Amount = model.Amount;
                //        existingModel.IncomeType = model.IncomeType;
                //        existingModel.Remark = model.Remark;
                //        existingModel.UpdatedAt = DateTime.Now;

                //        title = "Company Income Updated";
                //    }
                //}
                //else
                //{
                //    model.IncomeTitleName = incomeTileName;
                //    model.CreatedAt = DateTime.Now;
                //    _context.Income.Add(model);

                //    title = "Company Income Created";
                //}
                //await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your company income data has been saved.", ActionName = "CompanyIncome", ActionName2 = "IncomeList", BtnName="New Income", ControllerName = "CashManagement" });
            }
            var incomeTitle = await _context.IncomeTitle.AsNoTracking()
           .Select(x => new { x.Id, x.Name })
           .ToListAsync();

            if (incomeTitle == null || !incomeTitle.Any())
            {
                ViewData["IncomeTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["IncomeTitle"] = new SelectList(incomeTitle, "Id", "Name");
            }
            return View(model);
        }


        public async Task<IActionResult> IncomeList(DateTime? fromdate, DateTime? todate, int? incomeTitle)
        {
            var incomeTitleObj = await _context.IncomeTitle.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (incomeTitleObj == null || !incomeTitleObj.Any())
            {
                ViewData["IncomeTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["IncomeTitle"] = new SelectList(incomeTitleObj, "Id", "Name", incomeTitle);
            }

            // Pass the selected values back to the view
            ViewBag.FromDate = fromdate;
            ViewBag.ToDate = todate;
            ViewBag.IncomeTitleId = incomeTitle;

            if (fromdate == null && todate == null && incomeTitle == null)
            {
                return View(await _context.Income.AsNoTracking().ToListAsync());
            }
            else
            {
                var incomes = _context.Income.AsNoTracking().AsQueryable();

                if (fromdate.HasValue)
                {
                    incomes = incomes.Where(i => i.IncomeDate >= fromdate.Value);
                }

                if (todate.HasValue)
                {
                    incomes = incomes.Where(i => i.IncomeDate <= todate.Value);
                }

                if (incomeTitle.HasValue)
                {
                    incomes = incomes.Where(i => i.IncomeTitleId == incomeTitle.Value);
                }

                return View(await incomes.ToListAsync());
            }
        }
    }
}

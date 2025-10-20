using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.CashManagement;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OfficeOpenXml.Style;
using SimpleDataWebsite.Data;
using System;
using System.Security.Claims;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class CashManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashManagementController(ApplicationDbContext context)
        {
            _context=context;
        }

        #region Company Income
        public async Task<IActionResult> CompanyIncome(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

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

            var incomeTitle = await _context.IncomeTitle.AsNoTracking()
            .Where(x => x.CPI == cpi)
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
        public async Task<IActionResult> CompanyIncome(CompanyIncomeModel model)
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
                string title = "Company Income Created";
                string incomeTileName = (model.IncomeTitleId != null && model.IncomeTitleId > 0) ?
                    await _context.IncomeTitle.AsNoTracking().Where(x => x.Id == model.IncomeTitleId).Select(x => x.Name).FirstOrDefaultAsync()??""
                    : "";

                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";


                if (model.Id > 0)
                {
                    var existingModel = await _context.Income.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.IncomeDate = model.IncomeDate;
                        existingModel.IncomeTitleId = model.IncomeTitleId;
                        existingModel.IncomeTitleName =  incomeTileName;
                        existingModel.Amount = model.Amount;
                        existingModel.IncomeType = model.IncomeType;
                        existingModel.Remark = model.Remark;
                        existingModel.UpdatedAt = DateTime.Now;
                        existingModel.CPI = cpi;

                        title = "Company Income Updated";
                        log.Logs = $"{loginUserName} Update Income ({existingModel.Amount} to Cash) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                    }
                }
                else
                {
                    model.IncomeTitleName = incomeTileName;
                    model.CreatedAt = DateTime.Now;
                    model.CPI = cpi;
                    _context.Income.Add(model);

                    #region for cash book transaction
                    var newCashBookTransaction = new CashBookTransaction()
                    {
                        TransactionDate = model.IncomeDate,
                        Amount = model.Amount,
                        TransactionType = "Debit",
                        Description = $"Income To: Cash, Remark: {model.Remark}",
                        Account = "Cash",
                        CreatedAt = DateTime.Now,
                        CPI = cpi
                    };
                    _context.CashBookTransaction.Add(newCashBookTransaction);
                    #endregion
                    title = "Company Income Created";
                    log.Logs = $"{loginUserName} Add Income ({model.Amount} to Cash) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }

                #region log area
                log.CPI = cpi;
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "AccountRelated";

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your company income data has been saved.", ActionName = "CompanyIncome", ActionName2 = "IncomeList", BtnName = "New Income", ControllerName = "CashManagement" });
            }
            var incomeTitle = await _context.IncomeTitle.AsNoTracking()
                .Where(x => x.CPI == cpi)
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


        public async Task<IActionResult> IncomeList(DateTime? fromdate, DateTime? todate, string? incomeTitle)
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

            var incomeTitleObj = await _context.IncomeTitle.AsNoTracking()
                .Where(x => x.CPI == cpi)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (incomeTitleObj == null || !incomeTitleObj.Any())
            {
                ViewData["IncomeTitle"] = new SelectList(new List<object>(), "Name", "Name");
            }
            else
            {
                ViewData["IncomeTitle"] = new SelectList(incomeTitleObj, "Name", "Name", incomeTitle);
            }

            // Pass the selected values back to the view
            ViewBag.FromDate = fromdate;
            ViewBag.ToDate = todate;
            ViewBag.IncomeTitleId = incomeTitle;

            if (fromdate == null && todate == null && incomeTitle == null)
            {
                return View(await _context.Income.AsNoTracking().Where(x => x.CPI == cpi).ToListAsync());
            }
            else
            {
                var incomes = _context.Income.AsNoTracking().Where(x => x.CPI == cpi).AsQueryable();

                if (fromdate.HasValue)
                {
                    incomes = incomes.Where(i => i.IncomeDate >= fromdate.Value);
                }

                if (todate.HasValue)
                {
                    incomes = incomes.Where(i => i.IncomeDate <= todate.Value);
                }

                if (!string.IsNullOrEmpty(incomeTitle))
                {
                    incomes = incomes.Where(i => i.IncomeTitleName == incomeTitle);
                }

                return View(await incomes.ToListAsync());
            }
        }
        #endregion


        #region Company Expense
        public async Task<IActionResult> CompanyExpense(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

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

            var selectOptions = await _context.ExpenseTitle.AsNoTracking()
                .Where(x => x.CPI == cpi)
           .Select(x => new { x.Id, x.Name })
           .ToListAsync();

            if (selectOptions == null || !selectOptions.Any())
            {
                ViewData["ExpenseTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["ExpenseTitle"] = new SelectList(selectOptions, "Id", "Name");
            }
            if (id != null && id > 0)
            {
                return View(await _context.Expense.FindAsync(id));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompanyExpense(CompanyExpenseModel model)
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
                string title = "Company Expense Created";
                string expenseTileName = (model.ExpenseTitleId != null && model.ExpenseTitleId > 0) ?
                    await _context.ExpenseTitle.AsNoTracking().Where(x => x.Id == model.ExpenseTitleId).Select(x => x.Name).FirstOrDefaultAsync()??""
                    : "";

                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";

                if (model.Id > 0)
                {
                    var existingModel = await _context.Expense.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.ExpenseDate = model.ExpenseDate;
                        existingModel.ExpenseTitleId = model.ExpenseTitleId;
                        existingModel.ExpenseTitleName =  expenseTileName;
                        existingModel.Amount = model.Amount;
                        existingModel.ExpenseType = model.ExpenseType;
                        existingModel.Description = model.Description;
                        existingModel.UpdatedAt = DateTime.Now;
                        existingModel.CPI = cpi;

                        title = "Company Expense Updated";

                        log.Logs = $"{loginUserName} Update Expense ({existingModel.Amount} From Cash) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                    }
                }
                else
                {
                    model.ExpenseTitleName = expenseTileName;
                    model.CreatedAt = DateTime.Now;
                    model.CPI = cpi;

                    _context.Expense.Add(model);

                    #region for cash book transaction
                    var newCashBookTransaction = new CashBookTransaction()
                    {
                        TransactionDate = model.ExpenseDate,
                        Amount = model.Amount,
                        TransactionType = "Credit",
                        Description = $"Expense From: Cash, Remark: {model.Description}",
                        Account = "Cash",
                        CreatedAt = DateTime.Now,
                        CPI = cpi
                    };
                    _context.CashBookTransaction.Add(newCashBookTransaction);
                    #endregion
                    title = "Company Expense Created";
                    log.Logs = $"{loginUserName} Add Expense ({model.Amount} From Cash) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }
                #region log area
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "AccountRelated";
                log.CPI = cpi;

                _context.Log.Add(log);
                #endregion
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your company expense data has been saved.", ActionName = "CompanyExpense", ActionName2 = "ExpenseList", BtnName = "New Expense", ControllerName = "CashManagement" });
            }
            var incomeTitle = await _context.ExpenseTitle.AsNoTracking()
                .Where(x => x.CPI ==cpi)
           .Select(x => new { x.Id, x.Name })
           .ToListAsync();

            if (incomeTitle == null || !incomeTitle.Any())
            {
                ViewData["ExpenseTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["ExpenseTitle"] = new SelectList(incomeTitle, "Id", "Name");
            }
            return View(model);
        }


        public async Task<IActionResult> ExpenseList(DateTime? fromdate, DateTime? todate, int? expenseTitle)
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

            var expenseTitleObj = await _context.ExpenseTitle.AsNoTracking()
                .Where(x => x.CPI == cpi)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (expenseTitleObj == null || !expenseTitleObj.Any())
            {
                ViewData["ExpenseTitle"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["ExpenseTitle"] = new SelectList(expenseTitleObj, "Id", "Name", expenseTitle);
            }

            // Pass the selected values back to the view
            ViewBag.FromDate = fromdate;
            ViewBag.ToDate = todate;
            ViewBag.IncomeTitleId = expenseTitle;

            if (fromdate == null && todate == null && expenseTitle == null)
            {
                return View(await _context.Expense.AsNoTracking().Where(x => x.CPI == cpi).ToListAsync());
            }
            else
            {
                var expense = _context.Expense.AsNoTracking().Where(x => x.CPI == cpi).AsQueryable();

                if (fromdate.HasValue)
                {
                    expense = expense.Where(i => i.ExpenseDate >= fromdate.Value);
                }

                if (todate.HasValue)
                {
                    expense = expense.Where(i => i.ExpenseDate <= todate.Value);
                }

                if (expenseTitle.HasValue)
                {
                    expense = expense.Where(i => i.ExpenseTitleId == expenseTitle.Value);
                }

                return View(await expense.ToListAsync());
            }
        }
        #endregion

        public async Task<IActionResult> AccountTransfer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AccountTransfer(CashBookTransaction model)
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
                string fromtitle = "";
                string defDesc = "Account Transfer From: Main Cash, To: APM AYA Bank, Remark: ";
                string title = "Account transfer data added";
                model.Description = defDesc + model.Description;
                model.Account = model.TransactionType == "Debit" ? "APM AYA Bank" : "Cash";
                fromtitle = model.TransactionType == "Debit" ? "Cash" : "APM AYA Bank";
                model.CreatedAt = DateTime.Now;
                model.CPI = cpi;

                _context.CashBookTransaction.Add(model);

                #region log area
                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";
                log.Logs = $"{loginUserName} Add Transfer ({model.Amount} from {fromtitle} to {model.Account}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "AccountRelated";
                log.CPI = cpi;

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your account transfer data has been saved.", ActionName = "AccountTransfer", ActionName2 = "CashBookList", BtnName = "Account Transfer", ControllerName = "CashManagement" });
            }
            return View(model);
        }

        public async Task<IActionResult> CashBookList(DateTime? fromDate, DateTime? toDate)
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

            // Default to today if no date range is provided
            fromDate = fromDate ?? DateTime.Today;
            toDate = toDate ?? DateTime.Today;

            // Fetch transactions within the date range
            var transactions = await _context.CashBookTransaction
                .AsNoTracking()
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate && t.CPI == cpi)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            // Initialize balances
            double cashOpeningBalance = 0; // Fetch from a predefined starting balance if available
            double bankOpeningBalance = 0; // Fetch from a predefined starting balance if available

            var dateBalances = new List<DateBalanceModel>();

            // Loop through each day in the date range
            for (var date = fromDate.Value; date <= toDate.Value; date = date.AddDays(1))
            {
                // Filter transactions for the current date
                var dailyTransactions = transactions
                    .Where(t => t.TransactionDate.Date == date.Date)
                    .ToList();

                // Calculate daily totals for cash and bank
                double cashCredit = dailyTransactions
                    .Where(t => t.Account == "Cash" && t.TransactionType == "Credit")
                    .Sum(t => t.Amount);

                double cashDebit = dailyTransactions
                    .Where(t => t.Account == "Cash" && t.TransactionType == "Debit")
                    .Sum(t => t.Amount);

                double bankCredit = dailyTransactions
                    .Where(t => t.Account == "APM AYA Bank" && t.TransactionType == "Credit")
                    .Sum(t => t.Amount);

                double bankDebit = dailyTransactions
                    .Where(t => t.Account == "APM AYA Bank" && t.TransactionType == "Debit")
                    .Sum(t => t.Amount);

                // Calculate closing balances
                double cashClosingBalance = cashOpeningBalance + cashDebit - cashCredit;
                double bankClosingBalance = bankOpeningBalance + bankDebit - bankCredit;

                // Add to the list
                dateBalances.Add(new DateBalanceModel
                {
                    Date = date,
                    CashOpeningBalance = cashOpeningBalance,
                    CashClosingBalance = cashClosingBalance,
                    BankOpeningBalance = bankOpeningBalance,
                    BankClosingBalance = bankClosingBalance
                });

                // Set opening balances for the next day
                cashOpeningBalance = cashClosingBalance;
                bankOpeningBalance = bankClosingBalance;
            }

            // Pass data to the view
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(dateBalances);
        }
    }
}

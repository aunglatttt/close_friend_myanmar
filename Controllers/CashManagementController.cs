using CloseFriendMyanamr.Models.CashManagement;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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

        #region Company Income
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
        public async Task<IActionResult> CompanyIncome(CompanyIncomeModel model)
        {
            if (ModelState.IsValid)
            {
                string title = "Company Income Created";
                string incomeTileName = (model.IncomeTitleId != null && model.IncomeTitleId > 0) ?
                    await _context.IncomeTitle.AsNoTracking().Where(x => x.Id == model.IncomeTitleId).Select(x => x.Name).FirstOrDefaultAsync()??""
                    : "";

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

                        title = "Company Income Updated";
                    }
                }
                else
                {
                    model.IncomeTitleName = incomeTileName;
                    model.CreatedAt = DateTime.Now;
                    _context.Income.Add(model);

                    #region for cash book transaction
                    var newCashBookTransaction = new CashBookTransaction()
                    {
                        TransactionDate = model.IncomeDate,
                        Amount = model.Amount,
                        TransactionType = "Debit",
                        Descritpion = $"Income To: Cash, Remark: {model.Remark}",
                        Account = "Cash",
                        CreatedAt = DateTime.Now,
                    };
                    _context.CashBookTransaction.Add(newCashBookTransaction);
                    #endregion
                    title = "Company Income Created";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your company income data has been saved.", ActionName = "CompanyIncome", ActionName2 = "IncomeList", BtnName = "New Income", ControllerName = "CashManagement" });
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
        #endregion


        #region Company Expense
        public async Task<IActionResult> CompanyExpense(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            var selectOptions = await _context.ExpenseTitle.AsNoTracking()
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
            if (ModelState.IsValid)
            {
                string title = "Company Expense Created";
                string expenseTileName = (model.ExpenseTitleId != null && model.ExpenseTitleId > 0) ?
                    await _context.ExpenseTitle.AsNoTracking().Where(x => x.Id == model.ExpenseTitleId).Select(x => x.Name).FirstOrDefaultAsync()??""
                    : "";

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
                        existingModel.Remark = model.Remark;
                        existingModel.UpdatedAt = DateTime.Now;

                        title = "Company Expense Updated";
                    }
                }
                else
                {
                    model.ExpenseTitleName = expenseTileName;
                    model.CreatedAt = DateTime.Now;
                    _context.Expense.Add(model);

                    #region for cash book transaction
                    var newCashBookTransaction = new CashBookTransaction()
                    {
                        TransactionDate = model.ExpenseDate,
                        Amount = model.Amount,
                        TransactionType = "Credit",
                        Descritpion = $"Expense From: Cash, Remark: {model.Remark}",
                        Account = "Cash",
                        CreatedAt = DateTime.Now,
                    };
                    _context.CashBookTransaction.Add(newCashBookTransaction);
                    #endregion
                    title = "Company Expense Created";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your company expense data has been saved.", ActionName = "CompanyExpense", ActionName2 = "ExpenseList", BtnName = "New Expense", ControllerName = "CashManagement" });
            }
            var incomeTitle = await _context.ExpenseTitle.AsNoTracking()
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
            var expenseTitleObj = await _context.ExpenseTitle.AsNoTracking()
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
                return View(await _context.Expense.AsNoTracking().ToListAsync());
            }
            else
            {
                var expense = _context.Expense.AsNoTracking().AsQueryable();

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
            if (ModelState.IsValid)
            {
                string defDesc = "Account Transfer From: Main Cash, To: APM AYA Bank, Remark: ";
                string title = "Account transfer data added";
                model.Descritpion = defDesc + model.Descritpion;
                model.Account = model.TransactionType == "Debit" ? "APM AYA Bank" : "Cash";
                model.CreatedAt = DateTime.Now;
                _context.CashBookTransaction.Add(model);


                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessComponent", "Home", new { Title = title, Message = "Your account transfer data has been saved.", ActionName = "AccountTransfer", ActionName2 = "CashBookList", BtnName = "Account Transfer", ControllerName = "CashManagement" });
            }
            return View(model);
        }

        public IActionResult CashBookList(DateTime? fromDate, DateTime? toDate)
        {
            // Default to today if no date range is provided
            fromDate = fromDate ?? DateTime.Today;
            toDate = toDate ?? DateTime.Today;

            // Fetch transactions within the date range
            var transactions = _context.CashBookTransaction
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .OrderBy(t => t.TransactionDate)
                .ToList();

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

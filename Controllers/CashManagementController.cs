using System.Security.Claims;
using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.CashManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize(Roles = "Administrator,BOD")]
    public class CashManagementController : Controller
    {
        private const string CashAccount = "Cash";
        private const string BankAccount = "APM AYA Bank";
        private const string DebitTransaction = "Debit";
        private const string CreditTransaction = "Credit";
        private static readonly string[] SupportedAccounts = [CashAccount, BankAccount];

        private readonly ApplicationDbContext _context;

        public CashManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Company Income
        public async Task<IActionResult> CompanyIncome(int? id)
        {
            ViewBag.CreateOrUpdate = id.GetValueOrDefault() > 0 ? "Update" : "Create New";
            await PopulateIncomeTitleOptionsAsync();

            if (id.GetValueOrDefault() > 0)
            {
                var model = await _context.Income.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }

            return View(new CompanyIncomeModel
            {
                IncomeDate = DateTime.Today,
                IncomeType = CashAccount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyIncome(CompanyIncomeModel model)
        {
            ViewBag.CreateOrUpdate = model.Id > 0 ? "Update" : "Create New";
            model.IncomeType = CashAccount;
            model.Remark = model.Remark?.Trim();
            model.IncomeDate = model.IncomeDate.Date;

            if (!model.IncomeTitleId.HasValue || model.IncomeTitleId <= 0)
            {
                ModelState.AddModelError(nameof(model.IncomeTitleId), "Income title is required.");
            }

            if (model.Amount <= 0)
            {
                ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateIncomeTitleOptionsAsync(model.IncomeTitleId);
                return View(model);
            }

            string incomeTitleName = await _context.IncomeTitle.AsNoTracking()
                .Where(x => x.Id == model.IncomeTitleId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(incomeTitleName))
            {
                ModelState.AddModelError(nameof(model.IncomeTitleId), "Selected income title was not found.");
                await PopulateIncomeTitleOptionsAsync(model.IncomeTitleId);
                return View(model);
            }

            var (userId, loginUserName) = await GetCurrentEmployeeInfoAsync();
            var now = DateTime.Now;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            CompanyIncomeModel incomeEntity;
            CashBookTransaction? cashBookEntry;
            string title;

            if (model.Id > 0)
            {
                incomeEntity = await _context.Income.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (incomeEntity == null)
                {
                    return NotFound();
                }

                cashBookEntry = await FindIncomeCashTransactionAsync(incomeEntity);

                incomeEntity.IncomeDate = model.IncomeDate;
                incomeEntity.IncomeTitleId = model.IncomeTitleId;
                incomeEntity.IncomeTitleName = incomeTitleName;
                incomeEntity.Amount = model.Amount;
                incomeEntity.IncomeType = CashAccount;
                incomeEntity.Remark = model.Remark;
                incomeEntity.UpdatedAt = now;

                title = "Company Income Updated";
            }
            else
            {
                incomeEntity = new CompanyIncomeModel
                {
                    IncomeDate = model.IncomeDate,
                    IncomeTitleId = model.IncomeTitleId,
                    IncomeTitleName = incomeTitleName,
                    Amount = model.Amount,
                    IncomeType = CashAccount,
                    Remark = model.Remark,
                    CreatedAt = now
                };

                _context.Income.Add(incomeEntity);
                await _context.SaveChangesAsync();

                cashBookEntry = null;
                title = "Company Income Created";
            }

            if (cashBookEntry == null)
            {
                cashBookEntry = new CashBookTransaction
                {
                    CreatedAt = now
                };
                _context.CashBookTransaction.Add(cashBookEntry);
            }
            else
            {
                cashBookEntry.UpdatedAt = now;
            }

            cashBookEntry.TransactionDate = incomeEntity.IncomeDate.Date;
            cashBookEntry.Amount = incomeEntity.Amount;
            cashBookEntry.TransactionType = DebitTransaction;
            cashBookEntry.Account = CashAccount;
            cashBookEntry.Description = BuildIncomeTransactionDescription(incomeEntity.Id, incomeEntity.Remark);

            _context.Log.Add(new LogModel
            {
                EmployeeId = userId,
                LogsDate = now,
                Type = "AccountRelated",
                Logs = model.Id > 0
                    ? $"{loginUserName} Update Income ({incomeEntity.Amount} to {CashAccount}) @ {now:MMM dd, yyyy}"
                    : $"{loginUserName} Add Income ({incomeEntity.Amount} to {CashAccount}) @ {now:MMM dd, yyyy}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("SuccessComponent", "Home", new
            {
                Title = title,
                Message = "Your company income data has been saved.",
                ActionName = "CompanyIncome",
                ActionName2 = "IncomeList",
                BtnName = "New Income",
                ControllerName = "CashManagement"
            });
        }

        public async Task<IActionResult> IncomeList(DateTime? fromDate, DateTime? toDate, int? incomeTitleId)
        {
            await PopulateIncomeTitleOptionsAsync(incomeTitleId);

            ViewBag.FromDate = fromDate?.Date;
            ViewBag.ToDate = toDate?.Date;
            ViewBag.IncomeTitleId = incomeTitleId;

            var incomes = _context.Income.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                incomes = incomes.Where(i => i.IncomeDate >= from);
            }

            if (toDate.HasValue)
            {
                var toExclusive = toDate.Value.Date.AddDays(1);
                incomes = incomes.Where(i => i.IncomeDate < toExclusive);
            }

            if (incomeTitleId.HasValue)
            {
                incomes = incomes.Where(i => i.IncomeTitleId == incomeTitleId.Value);
            }

            return View(await incomes
                .OrderByDescending(i => i.IncomeDate)
                .ThenByDescending(i => i.Id)
                .ToListAsync());
        }
        #endregion

        #region Company Expense
        public async Task<IActionResult> CompanyExpense(int? id)
        {
            ViewBag.CreateOrUpdate = id.GetValueOrDefault() > 0 ? "Update" : "Create New";
            await PopulateExpenseTitleOptionsAsync();

            if (id.GetValueOrDefault() > 0)
            {
                var model = await _context.Expense.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }

            return View(new CompanyExpenseModel
            {
                ExpenseDate = DateTime.Today,
                ExpenseType = CashAccount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyExpense(CompanyExpenseModel model)
        {
            ViewBag.CreateOrUpdate = model.Id > 0 ? "Update" : "Create New";
            model.ExpenseType = CashAccount;
            model.Description = model.Description?.Trim();
            model.ExpenseDate = model.ExpenseDate.Date;

            if (!model.ExpenseTitleId.HasValue || model.ExpenseTitleId <= 0)
            {
                ModelState.AddModelError(nameof(model.ExpenseTitleId), "Expense title is required.");
            }

            if (model.Amount <= 0)
            {
                ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateExpenseTitleOptionsAsync(model.ExpenseTitleId);
                return View(model);
            }

            string expenseTitleName = await _context.ExpenseTitle.AsNoTracking()
                .Where(x => x.Id == model.ExpenseTitleId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(expenseTitleName))
            {
                ModelState.AddModelError(nameof(model.ExpenseTitleId), "Selected expense title was not found.");
                await PopulateExpenseTitleOptionsAsync(model.ExpenseTitleId);
                return View(model);
            }

            var (userId, loginUserName) = await GetCurrentEmployeeInfoAsync();
            var now = DateTime.Now;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            CompanyExpenseModel expenseEntity;
            CashBookTransaction? cashBookEntry;
            string title;

            if (model.Id > 0)
            {
                expenseEntity = await _context.Expense.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (expenseEntity == null)
                {
                    return NotFound();
                }

                cashBookEntry = await FindExpenseCashTransactionAsync(expenseEntity);

                expenseEntity.ExpenseDate = model.ExpenseDate;
                expenseEntity.ExpenseTitleId = model.ExpenseTitleId;
                expenseEntity.ExpenseTitleName = expenseTitleName;
                expenseEntity.Amount = model.Amount;
                expenseEntity.ExpenseType = CashAccount;
                expenseEntity.Description = model.Description;
                expenseEntity.UpdatedAt = now;

                title = "Company Expense Updated";
            }
            else
            {
                expenseEntity = new CompanyExpenseModel
                {
                    ExpenseDate = model.ExpenseDate,
                    ExpenseTitleId = model.ExpenseTitleId,
                    ExpenseTitleName = expenseTitleName,
                    Amount = model.Amount,
                    ExpenseType = CashAccount,
                    Description = model.Description,
                    CreatedAt = now
                };

                _context.Expense.Add(expenseEntity);
                await _context.SaveChangesAsync();

                cashBookEntry = null;
                title = "Company Expense Created";
            }

            if (cashBookEntry == null)
            {
                cashBookEntry = new CashBookTransaction
                {
                    CreatedAt = now
                };
                _context.CashBookTransaction.Add(cashBookEntry);
            }
            else
            {
                cashBookEntry.UpdatedAt = now;
            }

            cashBookEntry.TransactionDate = expenseEntity.ExpenseDate.Date;
            cashBookEntry.Amount = expenseEntity.Amount;
            cashBookEntry.TransactionType = CreditTransaction;
            cashBookEntry.Account = CashAccount;
            cashBookEntry.Description = BuildExpenseTransactionDescription(expenseEntity.Id, expenseEntity.Description);

            _context.Log.Add(new LogModel
            {
                EmployeeId = userId,
                LogsDate = now,
                Type = "AccountRelated",
                Logs = model.Id > 0
                    ? $"{loginUserName} Update Expense ({expenseEntity.Amount} from {CashAccount}) @ {now:MMM dd, yyyy}"
                    : $"{loginUserName} Add Expense ({expenseEntity.Amount} from {CashAccount}) @ {now:MMM dd, yyyy}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("SuccessComponent", "Home", new
            {
                Title = title,
                Message = "Your company expense data has been saved.",
                ActionName = "CompanyExpense",
                ActionName2 = "ExpenseList",
                BtnName = "New Expense",
                ControllerName = "CashManagement"
            });
        }

        public async Task<IActionResult> ExpenseList(DateTime? fromDate, DateTime? toDate, int? expenseTitle)
        {
            await PopulateExpenseTitleOptionsAsync(expenseTitle);

            ViewBag.FromDate = fromDate?.Date;
            ViewBag.ToDate = toDate?.Date;
            ViewBag.ExpenseTitleId = expenseTitle;

            var expenses = _context.Expense.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                expenses = expenses.Where(i => i.ExpenseDate >= from);
            }

            if (toDate.HasValue)
            {
                var toExclusive = toDate.Value.Date.AddDays(1);
                expenses = expenses.Where(i => i.ExpenseDate < toExclusive);
            }

            if (expenseTitle.HasValue)
            {
                expenses = expenses.Where(i => i.ExpenseTitleId == expenseTitle.Value);
            }

            return View(await expenses
                .OrderByDescending(i => i.ExpenseDate)
                .ThenByDescending(i => i.Id)
                .ToListAsync());
        }
        #endregion

        public IActionResult AccountTransfer()
        {
            return View(new AccountTransferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountTransfer(AccountTransferViewModel model)
        {
            model.FromAccount = model.FromAccount?.Trim();
            model.ToAccount = model.ToAccount?.Trim();
            model.Description = model.Description?.Trim();
            model.TransactionDate = model.TransactionDate.Date;

            if (model.Amount <= 0)
            {
                ModelState.AddModelError(nameof(model.Amount), "Amount must be greater than zero.");
            }

            if (!IsSupportedAccount(model.FromAccount))
            {
                ModelState.AddModelError(nameof(model.FromAccount), "Selected source account is not valid.");
            }

            if (!IsSupportedAccount(model.ToAccount))
            {
                ModelState.AddModelError(nameof(model.ToAccount), "Selected destination account is not valid.");
            }

            if (string.Equals(model.FromAccount, model.ToAccount, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.ToAccount), "From account and to account must be different.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (userId, loginUserName) = await GetCurrentEmployeeInfoAsync();
            var now = DateTime.Now;
            var transferReference = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var remarkSuffix = string.IsNullOrWhiteSpace(model.Description) ? string.Empty : $", Remark: {model.Description}";

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.CashBookTransaction.AddRange(
                new CashBookTransaction
                {
                    TransactionDate = model.TransactionDate,
                    Amount = model.Amount,
                    TransactionType = CreditTransaction,
                    Account = model.FromAccount,
                    Description = $"[Transfer:{transferReference}] Transfer To: {model.ToAccount}{remarkSuffix}",
                    CreatedAt = now
                },
                new CashBookTransaction
                {
                    TransactionDate = model.TransactionDate,
                    Amount = model.Amount,
                    TransactionType = DebitTransaction,
                    Account = model.ToAccount,
                    Description = $"[Transfer:{transferReference}] Transfer From: {model.FromAccount}{remarkSuffix}",
                    CreatedAt = now
                });

            _context.Log.Add(new LogModel
            {
                EmployeeId = userId,
                LogsDate = now,
                Type = "AccountRelated",
                Logs = $"{loginUserName} Add Transfer ({model.Amount} from {model.FromAccount} to {model.ToAccount}) @ {now:MMM dd, yyyy}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("SuccessComponent", "Home", new
            {
                Title = "Account Transfer Created",
                Message = "Your account transfer data has been saved.",
                ActionName = "AccountTransfer",
                ActionName2 = "CashBookList",
                BtnName = "New Transfer",
                ControllerName = "CashManagement"
            });
        }

        public async Task<IActionResult> CashBookList(DateTime? fromDate, DateTime? toDate)
        {
            var normalizedFromDate = (fromDate ?? DateTime.Today).Date;
            var normalizedToDate = (toDate ?? normalizedFromDate).Date;

            if (normalizedFromDate > normalizedToDate)
            {
                (normalizedFromDate, normalizedToDate) = (normalizedToDate, normalizedFromDate);
            }

            var openingTransactions = await _context.CashBookTransaction
                .AsNoTracking()
                .Where(t => t.TransactionDate < normalizedFromDate)
                .ToListAsync();

            var transactions = await _context.CashBookTransaction
                .AsNoTracking()
                .Where(t => t.TransactionDate >= normalizedFromDate && t.TransactionDate < normalizedToDate.AddDays(1))
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.Id)
                .ToListAsync();

            decimal cashOpeningBalance = CalculateClosingBalance(openingTransactions, CashAccount);
            decimal bankOpeningBalance = CalculateClosingBalance(openingTransactions, BankAccount);

            var dateBalances = new List<DateBalanceModel>();

            for (var date = normalizedFromDate; date <= normalizedToDate; date = date.AddDays(1))
            {
                var dailyTransactions = transactions
                    .Where(t => t.TransactionDate.Date == date.Date)
                    .ToList();

                decimal cashCredit = dailyTransactions
                    .Where(t => t.Account == CashAccount && t.TransactionType == CreditTransaction)
                    .Sum(t => (decimal)t.Amount);

                decimal cashDebit = dailyTransactions
                    .Where(t => t.Account == CashAccount && t.TransactionType == DebitTransaction)
                    .Sum(t => (decimal)t.Amount);

                decimal bankCredit = dailyTransactions
                    .Where(t => t.Account == BankAccount && t.TransactionType == CreditTransaction)
                    .Sum(t => (decimal)t.Amount);

                decimal bankDebit = dailyTransactions
                    .Where(t => t.Account == BankAccount && t.TransactionType == DebitTransaction)
                    .Sum(t => (decimal)t.Amount);

                decimal cashClosingBalance = cashOpeningBalance + cashDebit - cashCredit;
                decimal bankClosingBalance = bankOpeningBalance + bankDebit - bankCredit;

                dateBalances.Add(new DateBalanceModel
                {
                    Date = date,
                    CashOpeningBalance = cashOpeningBalance,
                    CashClosingBalance = cashClosingBalance,
                    BankOpeningBalance = bankOpeningBalance,
                    BankClosingBalance = bankClosingBalance
                });

                cashOpeningBalance = cashClosingBalance;
                bankOpeningBalance = bankClosingBalance;
            }

            ViewBag.FromDate = normalizedFromDate;
            ViewBag.ToDate = normalizedToDate;

            return View(dateBalances);
        }

        private async Task PopulateIncomeTitleOptionsAsync(object? selectedValue = null)
        {
            var incomeTitles = await _context.IncomeTitle.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewData["IncomeTitle"] = incomeTitles.Any()
                ? new SelectList(incomeTitles, "Id", "Name", selectedValue)
                : new SelectList(new List<object>(), "Id", "Name");
        }

        private async Task PopulateExpenseTitleOptionsAsync(object? selectedValue = null)
        {
            var expenseTitles = await _context.ExpenseTitle.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewData["ExpenseTitle"] = expenseTitles.Any()
                ? new SelectList(expenseTitles, "Id", "Name", selectedValue)
                : new SelectList(new List<object>(), "Id", "Name");
        }

        private async Task<(int UserId, string UserName)> GetCurrentEmployeeInfoAsync()
        {
            int userId = int.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedUserId)
                ? parsedUserId
                : 0;

            string loginUserName = await _context.Employee.AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.EmployeeName)
                .FirstOrDefaultAsync() ?? string.Empty;

            return (userId, loginUserName);
        }

        private async Task<CashBookTransaction?> FindIncomeCashTransactionAsync(CompanyIncomeModel income)
        {
            var marker = $"[Income:{income.Id}]";

            var transaction = await _context.CashBookTransaction.FirstOrDefaultAsync(t =>
                t.Account == CashAccount &&
                t.TransactionType == DebitTransaction &&
                t.Description != null &&
                t.Description.Contains(marker));

            if (transaction != null)
            {
                return transaction;
            }

            var dayStart = income.IncomeDate.Date;
            var dayEnd = dayStart.AddDays(1);
            var legacyDescription = BuildLegacyIncomeTransactionDescription(income.Remark);

            return await _context.CashBookTransaction
                .Where(t => t.Account == CashAccount &&
                            t.TransactionType == DebitTransaction &&
                            t.TransactionDate >= dayStart &&
                            t.TransactionDate < dayEnd &&
                            t.Amount == income.Amount &&
                            t.Description == legacyDescription)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<CashBookTransaction?> FindExpenseCashTransactionAsync(CompanyExpenseModel expense)
        {
            var marker = $"[Expense:{expense.Id}]";

            var transaction = await _context.CashBookTransaction.FirstOrDefaultAsync(t =>
                t.Account == CashAccount &&
                t.TransactionType == CreditTransaction &&
                t.Description != null &&
                t.Description.Contains(marker));

            if (transaction != null)
            {
                return transaction;
            }

            var dayStart = expense.ExpenseDate.Date;
            var dayEnd = dayStart.AddDays(1);
            var legacyDescription = BuildLegacyExpenseTransactionDescription(expense.Description);

            return await _context.CashBookTransaction
                .Where(t => t.Account == CashAccount &&
                            t.TransactionType == CreditTransaction &&
                            t.TransactionDate >= dayStart &&
                            t.TransactionDate < dayEnd &&
                            t.Amount == expense.Amount &&
                            t.Description == legacyDescription)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();
        }

        private static decimal CalculateClosingBalance(IEnumerable<CashBookTransaction> transactions, string account)
        {
            decimal totalDebit = transactions
                .Where(t => t.Account == account && t.TransactionType == DebitTransaction)
                .Sum(t => (decimal)t.Amount);

            decimal totalCredit = transactions
                .Where(t => t.Account == account && t.TransactionType == CreditTransaction)
                .Sum(t => (decimal)t.Amount);

            return totalDebit - totalCredit;
        }

        private static bool IsSupportedAccount(string? account)
        {
            return SupportedAccounts.Any(x => string.Equals(x, account, StringComparison.OrdinalIgnoreCase));
        }

        private static string BuildIncomeTransactionDescription(int incomeId, string? remark)
        {
            return $"[Income:{incomeId}] {BuildLegacyIncomeTransactionDescription(remark)}";
        }

        private static string BuildLegacyIncomeTransactionDescription(string? remark)
        {
            return $"Income To: {CashAccount}, Remark: {remark?.Trim()}";
        }

        private static string BuildExpenseTransactionDescription(int expenseId, string? description)
        {
            return $"[Expense:{expenseId}] {BuildLegacyExpenseTransactionDescription(description)}";
        }

        private static string BuildLegacyExpenseTransactionDescription(string? description)
        {
            return $"Expense From: {CashAccount}, Remark: {description?.Trim()}";
        }
    }
}

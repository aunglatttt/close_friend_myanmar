using CloseFriendMyanamr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class IncomeTitleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeTitleController(ApplicationDbContext context)
        {
            _context=context;
        }

        #region Income Titles
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

            ViewData["FormTitle"] = "Add Income Title";
            var items = await _context.IncomeTitle.AsNoTracking().Where(x => x.CPI == cpi).ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> Add(IncomeTitleModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required");


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

            item.CreatedAt = DateTime.Now;
            item.CPI = cpi;
            _context.IncomeTitle.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> Update(IncomeTitleModel item)
        {
            var existingItem = await _context.IncomeTitle.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            existingItem.Remark = item.Remark;
            existingItem.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.IncomeTitle.FindAsync(id);
            if (item == null) return NotFound();

            _context.IncomeTitle.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var epmtype = await _context.IncomeTitle.FindAsync(id);
            return Json(epmtype);
        }
        #endregion


        #region Expense Titles
        public async Task<IActionResult> ExpenseList()
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

            ViewData["FormTitle"] = "Add Expense";
            var items = await _context.ExpenseTitle.AsNoTracking().Where(x => x.CPI == cpi).ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddExpense(ExpenseTitleModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required");

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

            item.CPI = cpi;
            item.CreatedAt = DateTime.Now;
            _context.ExpenseTitle.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExpense(ExpenseTitleModel item)
        {
            var existingItem = await _context.ExpenseTitle.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            existingItem.Remark = item.Remark;
            existingItem.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var item = await _context.ExpenseTitle.FindAsync(id);
            if (item == null) return NotFound();

            _context.ExpenseTitle.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailExpense(int id)
        {
            var epmtype = await _context.ExpenseTitle.FindAsync(id);
            return Json(epmtype);
        }
        #endregion

        #region Facilities
        public async Task<IActionResult> FacilityList()
        {
            ViewData["FormTitle"] = "Add Facility";
            var items = await _context.Facilities.ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddFacility(FacilityModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required");

            item.CreatedAt = DateTime.Now;
            _context.Facilities.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFacility(FacilityModel item)
        {
            var existingItem = await _context.Facilities.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var item = await _context.Facilities.FindAsync(id);
            if (item == null) return NotFound();

            _context.Facilities.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailFacility(int id)
        {
            var epmtype = await _context.Facilities.FindAsync(id);
            return Json(epmtype);
        }
        #endregion

        #region Property Type
        public async Task<IActionResult> PropertyTypeList()
        {
            ViewData["FormTitle"] = "Add Property Type";
            var items = await _context.PropertyType.ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddPropertyType(PropertyTypeModel item)
        {
            if (string.IsNullOrWhiteSpace(item.TypeName))
                return BadRequest("TypeName is required");

            if (string.IsNullOrWhiteSpace(item.ShortCode))
                return BadRequest("ShortCode is required");

            bool isDuplicate = await _context.PropertyType.AsNoTracking()
                .AnyAsync(x =>
                
                    x.ShortCode.ToLower() == item.ShortCode.ToLower()
                );
            if (isDuplicate == true)
            {
                ViewData["ErrorMessage"] = $"Short Code '{item.ShortCode}' is already exists.";
                return BadRequest($"Short Code {item.ShortCode} is already exists.");
            }
            

            item.CreatedAt = DateTime.Now;
            _context.PropertyType.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePropertyType(PropertyTypeModel item)
        {
            var existingItem = await _context.PropertyType.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.TypeName = item.TypeName;
            existingItem.ShortCode = item.ShortCode;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePropertyType(int id)
        {
            var item = await _context.PropertyType.FindAsync(id);
            if (item == null) return NotFound();

            _context.PropertyType.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailPropertyType(int id)
        {
            var epmtype = await _context.PropertyType.FindAsync(id);
            return Json(epmtype);
        }
        #endregion

        #region Building Type
        public async Task<IActionResult> BuildingTypeList()
        {
            ViewData["FormTitle"] = "Add Building Type";
            var items = await _context.BuildingType.ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddBuildingType(BuildingTypeModel item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required");

            item.CreatedAt = DateTime.Now;
            _context.BuildingType.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBuildingType(BuildingTypeModel item)
        {
            var existingItem = await _context.BuildingType.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBuildingType(int id)
        {
            var item = await _context.BuildingType.FindAsync(id);
            if (item == null) return NotFound();

            _context.BuildingType.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailBuildingType(int id)
        {
            var epmtype = await _context.BuildingType.FindAsync(id);
            return Json(epmtype);
        }
        #endregion

        #region Bank Account Info
        public async Task<IActionResult> BankAccountInfoList()
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

            ViewData["FormTitle"] = "Add Bank Account Info";
            var items = await _context.BankAccount.AsNoTracking().Where(x => x.CPI == cpi).ToListAsync();
            return View(items);
        }


        [HttpPost]
        public async Task<IActionResult> AddBankAccountInfo(BankAccountModel item)
        {
            if (string.IsNullOrWhiteSpace(item.BankAccount))
                return BadRequest("BankName is required");

            //if (item.OpeningAmount <= 0)
            //    return BadRequest("OpeningAmount must be a valid positive number.");

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

            item.CPI = cpi;
            item.CreatedAt = DateTime.Now;
            _context.BankAccount.Add(item);
            await _context.SaveChangesAsync();
            return Json(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBankAccountInfo(BankAccountModel item)
        {
            var existingItem = await _context.BankAccount.FindAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.BankAccount = item.BankAccount;
            existingItem.OpeningAmount = item.OpeningAmount;
            existingItem.Remark = item.Remark;
            existingItem.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(existingItem);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBankAccountInfo(int id)
        {
            var item = await _context.BankAccount.FindAsync(id);
            if (item == null) return NotFound();

            _context.BankAccount.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailBankAccountInfo(int id)
        {
            var epmtype = await _context.BankAccount.FindAsync(id);
            return Json(epmtype);
        }
        #endregion



        public async Task<IActionResult> LogList(int id)
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
            return View(await _context.Log.AsNoTracking().Where(x => x.CPI == cpi).OrderByDescending(x => x.LogsDate).Include(x => x.Employee).ToListAsync());
        }
    }
}

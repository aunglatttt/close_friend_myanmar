using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloseFriendMyanamr.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.LoginError = "Please enter credentials.";
                return View();
            }

            string userAgent = Request.Headers["User-Agent"].ToString();
            bool isOldMobileApp = userAgent.Contains("MyCustomApp");
            bool isMobileApp = userAgent.Contains("CFMCustomApp");

            if (isOldMobileApp)
            {
                ViewBag.LoginError = "Your app version is no longer supported. Please update to the latest version to continue.";
                return View();
            }


            var client = await _context.Client.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClientPhone == username && x.Password == password);

            var employee = await _context.Employee.AsNoTracking()
                .Include(x => x.EmployeeType)
                .FirstOrDefaultAsync(x => x.LoginName == username && x.Password == password);

            if (isMobileApp && client != null && employee != null)
            {
                // Temporary storage to remember the choice during the next step
                TempData["Username"] = username;
                TempData["Password"] = password;
                TempData["RememberMe"] = rememberMe;
                return View("MobileRoleSelection");
            }

            if (client != null && employee == null)
            {
                return await ProcessClientLogin(client, rememberMe);
            }

            // CASE 3: ONLY EMPLOYEE OR (Web + Employee/Both)
            if (employee != null)
            {
                // On Web, if both exist, we default to Admin or you can handle differently
                return await ProcessEmployeeLogin(employee, rememberMe, returnUrl);
            }

            ViewBag.LoginError = "Invalid Login Information!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SelectRole(string role)
        {
            string username = TempData["Username"]?.ToString();
            string password = TempData["Password"]?.ToString();
            bool rememberMe = (bool)(TempData["RememberMe"] ?? false);

            if (role == "Client")
            {
                var user = await _context.Client.FirstOrDefaultAsync(x => x.ClientPhone == username && x.Password == password);
                return await ProcessClientLogin(user, rememberMe);
            }
            else
            {
                var user = await _context.Employee.Include(x => x.EmployeeType).FirstOrDefaultAsync(x => x.LoginName == username && x.Password == password);
                return await ProcessEmployeeLogin(user, rememberMe, null);
            }
        }

        private async Task<IActionResult> ProcessClientLogin(ClientModel user, bool rememberMe)
        {
            if (user.Status == "Block") { ViewBag.LoginError = "Account Blocked"; return View("Login"); }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.ClientName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "MobileUser")
            };
            await SignIn(claims, rememberMe);
            return RedirectToAction("Welcome", "Home");
        }

        private async Task<IActionResult> ProcessEmployeeLogin(EmployeeModel user, bool rememberMe, string returnUrl)
        {
            if (user.Status == false) { ViewBag.LoginError = "Account Inactive"; return View("Login"); }

            var claims = BuildEmployeeClaims(user);
            await SignIn(claims, rememberMe);
            return RedirectToAction(user.EmployeeTypeId == 4 ? "Welcome" : "Index", "Home");
        }

        private static List<Claim> BuildEmployeeClaims(EmployeeModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.EmployeeName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var role in GetEmployeeRoleClaims(user.EmployeeType?.Type))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private static IEnumerable<string> GetEmployeeRoleClaims(string? employeeType)
        {
            var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var primaryRole = string.IsNullOrWhiteSpace(employeeType) ? "Admin" : employeeType.Trim();

            roles.Add(primaryRole);

            // BOD should inherit the full Administrator surface area.
            if (string.Equals(primaryRole, "BOD", StringComparison.OrdinalIgnoreCase))
            {
                roles.Add("Administrator");
            }

            return roles;
        }

        private async Task SignIn(List<Claim> claims, bool isPersistent)
        {
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = isPersistent });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MobileRoleSelection()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existClient = await _context.Client.AsNoTracking()
        .FirstOrDefaultAsync(x => x.ClientPhone == NormalizeMyanmarPhone(model.PhoneNumber));

            if (existClient != null)
            {
                // attach error to PhoneNumber field
                ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                return View(model);
            }


            #region new method
            var newClient = new ClientModel
            {
                ClientName = model.FullName,
                ClientPhone = NormalizeMyanmarPhone(model.PhoneNumber),
                Address = model.Address,
                RegistrationDate = DateTime.Now,
                Status = "New",
                Remark = "Mobile User",
                Password = model.Password
            };

            await _context.Client.AddAsync(newClient);
            await _context.SaveChangesAsync();
            #endregion

            return RedirectToAction("Login");
        }

        private string NormalizeMyanmarPhone(string phone)
        {
            phone = phone.Replace(" ", "").Replace("-", "");

            if (phone.StartsWith("+9509"))
                phone = phone.Replace("+9509", "09");
            else if (phone.StartsWith("+95"))
                phone = phone.Replace("+95", "0");
            else if (phone.StartsWith("959"))
                phone = phone.Replace("959", "09");
            else if (phone.StartsWith("9"))
                phone = "0" + phone;

            return phone;
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // Example: get phone from session
            string userAgent = Request.Headers["User-Agent"].ToString();
            int userId = 0;
            var userIdObj = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdObj, out userId);



            if (userId <= 0)
            {
                return RedirectToAction("Login");
            }

            var client = await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (client == null)
            {
                return RedirectToAction("Login");
            }

            return View(client);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string userAgent = Request.Headers["User-Agent"].ToString();
            int userId = 0;
            var userIdObj = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdObj, out userId);



            if (userId <= 0)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Client.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null) return RedirectToAction("Login");

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("", "Current password is incorrect");
                return View(model);
            }

            user.Password = model.NewPassword;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Password updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}

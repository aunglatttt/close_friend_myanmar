using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authentication;
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
            // Validate the username and password (this is just a simple example)
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                string userAgent = Request.Headers["User-Agent"].ToString();
                if (userAgent.Contains("MyCustomApp"))
                {
                    var user = await _context.Client.AsNoTracking().Where(x => x.ClientPhone == username && x.Password == password).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        ViewBag.LoginError = "Invalid Login Information!";
                        return View();
                    }
                    else if (user.Status == "Block")
                    {
                        ViewBag.LoginError = "Your Account status is inactive.!";
                        return View();
                    }

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.ClientName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id + ""),
                    new Claim(ClaimTypes.Role, "MobileUser")
                };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe // Set the "Remember Me" option
                    };

                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                 
                        return RedirectToAction("Welcome", "Home");
             
                }
                else
                {
                    var user = await _context.Employee.AsNoTracking().Include(x => x.EmployeeType).Where(x => x.LoginName == username && x.Password == password).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        ViewBag.LoginError = "Invalid Login Information!";
                        return View();
                    }
                    else if (user.Status == false)
                    {
                        ViewBag.LoginError = "Your Account status is inactive.!";
                        return View();
                    }

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.EmployeeName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id + ""),
                    new Claim(ClaimTypes.Role, user.EmployeeType?.Type?? "Admin")
                };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe // Set the "Remember Me" option
                    };

                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (user.EmployeeTypeId == 4)
                            return RedirectToAction("Welcome", "Home");
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }

            }

            ViewBag.LoginError = "Invalid Login Information!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
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

            #region  old method
            // var newEmploy = new EmployeeModel
            // {
            //     EmployeeName = model.FullName,
            //     PhoneNumber = NormalizeMyanmarPhone(model.PhoneNumber),
            //     LoginName = NormalizeMyanmarPhone(model.PhoneNumber),
            //     Password = model.Password,
            //     EmployeeTypeId = 4,
            //     Status = true,
            //     CreatedAt = DateTime.Now
            // };

            // await _context.Employee.AddAsync(newEmploy);
            // await _context.SaveChangesAsync();
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

    }
}

using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;
using System.Security.Claims;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserManagementController(ApplicationDbContext context)
        {
            _context=context;
        }

        #region client
        public async Task<IActionResult> ClientList()
        {
            //var model = await _context.Client.AsNoTracking()
            //    .Select(x => new
            //    {
            //        x.Id,
            //        x.ClientName,
            //        x.ClientPhone,
            //        x.Address,
            //        x.RegistrationDate,
            //        x.Status,
            //        x.ShownProperty = x.ClientRequirements.Count(),
            //        x.Remark,
            //    })
                
            return View(await _context.Client.AsNoTracking().Include(x => x.ClientRequirements).ToListAsync());
        }

        public async Task<IActionResult> ClientCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                //var requirementObj = await _context.ClientRequirement.AsNoTracking()
                //    .Where(x => x.ClientId == id)
                //    .ToListAsync();

                var client = await _context.Client.AsNoTracking().Include(x => x.ClientRequirements).FirstOrDefaultAsync(x => x.Id == id);
                //if (client != null)
                //{
                //    client.ClientRequirements = new List<string>();
                //    client.ClientRequirements.Add("#1: (Condo အဝယ်)     New\r\nBudget >>> သိန်း 4000 မှ သိန်း 6000 အတွင်း\r\nလိုချင်သည့် အမျိုးအစား >>> RC (????)\r\nလိုချင်သည့် မြို့နယ် >>> သန်လျင်\r\nလမ်း >>> Housing streetရပ်ကွက် >>> Aurora\r\n\r\nအခြားလိုအပ်ချက်များ\r\n  >>> Master အနည်းဆုံး (1)ခန်း၊\r\n  >>> အကျယ် အနည်းဆုံး (1200) ရှိရပါမယ်\r\n  >>> အလွှာ အနိမ့်ဆုံး (6)လွှာ   >>> အလွှာ အမြင့်ဆုံး (16) ရှိရပါမယ်\r\n  >>> Swimming Pool, Balcony, Elevator, 24hrs Electricity, 24hrs Security, Car Parking\r\n\r\n\r\n");
                //}
                return View(client);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientCreate(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";
                if (model.Id > 0)
                {
                    var existingModel = await _context.Client.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.ClientName = model.ClientName;
                        existingModel.ClientPhone = model.ClientPhone;
                        existingModel.Address =  model.Address;
                        existingModel.RegistrationDate = model.RegistrationDate;
                        existingModel.Remark = model.Remark;
                        existingModel.UpdatedAt = DateTime.Now;
                        existingModel.Status = model.Status;

                        returnMsg = "Updated";
                        log.Logs = $"{loginUserName} Modify Client ({existingModel.ClientName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                    }
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    _context.Client.Add(model);

                    returnMsg = "Created";
                    log.Logs = $"{loginUserName} Add new Client ({model.ClientName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }

                #region log area
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "ClientRelated";

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessClient", new { message = returnMsg });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var item = await _context.Client.FindAsync(id);
            if (item == null) return NotFound();

            _context.Client.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public IActionResult SuccessClient(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        #endregion

        #region Owner
        public async Task<IActionResult> OwnerList()
        {
            var model = await _context.Owner.AsNoTracking()
                .Where(x => x.Type == "1")
                .Select(x => new AOViewModel
                {
                    Id = x.Id,
                    OwnerName = x.OwnerName,
                    OwnerPhone = x.OwnerPhone,
                    Address = x.Address,
                    Remark = x.Remark,
                    PropertyCount = x.Properties.Count()
                }).ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> OwnerCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                var model = await _context.Owner.Include(x => x.Properties).ThenInclude(x => x.LastCheckedBy).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (model != null && model.Properties.Any())
                {
                    foreach(var item in model.Properties)
                    {
                        item.PropertyType = await _context.PropertyType.AsNoTracking().Where(x => x.ShortCode == item.PropertyType).Select(x => x.TypeName).FirstOrDefaultAsync()??item.PropertyType;

                        if (item.BuildingType != null)
                        {
                            int buildingTypeId = int.TryParse(item.BuildingType, out int result) ? result : 0;
                            item.BuildingType = await _context.BuildingType.AsNoTracking().Where(x => x.Id == buildingTypeId).Select(x => x.Name).FirstOrDefaultAsync()??item.BuildingType;
                        }

                        #region ownership

                        switch (item.SalerOwnType)
                        {
                            case 1:
                                item.SalerOwnTypeString = "အမည်ပေါက်";
                                break;
                            case 2:
                                item.SalerOwnTypeString = "Special Power";
                                break;
                            case 3:
                                item.SalerOwnTypeString = "General Owner";
                                break;
                            case 4:
                                item.SalerOwnTypeString = "အမွေဆက်ခံ";
                                break;
                            case 5:
                                item.SalerOwnTypeString = "ဂရံဂတုံး";
                                break;
                            case 6:
                                item.SalerOwnTypeString = "Other";
                                break;
                            case 7:
                                item.SalerOwnTypeString = "အဆက်ဆက်စာချုပ်";
                                break;
                            default:
                                item.SalerOwnTypeString = item.SalerOwnType + "";
                                break;
                        }

                        switch (item.Ownership)
                        {
                            case "Grant_Original":
                                item.Ownership = "Grant (မူရင်း)";
                                break;
                            case "Grant_Copy":
                                item.Ownership = "Grant (မိတ္တူ)";
                                break;
                            case "Permit_Original":
                                item.Ownership = "Permit (မူရင်း)";
                                break;
                            case "Permit_Copy":
                                item.Ownership = "Permit (မိတ္တူ)";
                                break;
                            case "FreeHoldLand":
                                item.Ownership = "FreeHold Land";
                                break;
                            case "BCC":
                                item.Ownership = "BCC";
                                break;
                            case "Contract":
                                item.Ownership = "အစဉ်အဆက်စာချုပ်";
                                break;
                            case "Form_7":
                                item.Ownership = "Form 7 (ပုံစံ ၇)";
                                break;
                            default:
                                item.Ownership = item.Ownership;
                                break;
                        }


                        #endregion

                        #region commisssion
                        switch (item.SaleCommission)
                        {
                            case 1:
                                item.SaleCommissionString = "2% (အပြည့်အဝ)";
                                break;
                            case 2:
                                item.SaleCommissionString = "1% (တစ်ဝက်)";
                                break;
                            case 3:
                                item.SaleCommissionString = "1/3 (သုံးပုံတစ်ပုံ)";
                                break;
                            case 4:
                                item.SaleCommissionString = "အကျိုးတူ (အညီမျှ)";
                                break;
                            case 5:
                                item.SaleCommissionString = "အဆင်ပြေသလို ညှိယူရန်";
                                break;
                            default:
                                item.SaleCommissionString = "";
                                break;
                        }

                        switch (item.RentCommision)
                        {
                            case 1:
                                item.RentCommisionString = "၁လစာ (အိမ်ရှင်+အိမ်ငှား)";
                                break;
                            case 2:
                                item.RentCommisionString = "၁လစာ (တစ်ခြမ်း)";
                                break;
                            case 3:
                                item.RentCommisionString = "1/3 (သုံးပုံတစ်ပုံ)";
                                break;
                            case 4:
                                item.RentCommisionString = "အကျိုးတူ (အညီမျှ)";
                                break;
                            case 5:
                                item.RentCommisionString = "အဆင်ပြေသလို ညှိယူရန်";
                                break;
                            default:
                                item.RentCommisionString = "";
                                break;
                        }
                        #endregion
                    }

                }
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OwnerCreate(OwnerModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";
                if (model.Id > 0)
                {
                    var existingModel = await _context.Owner.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.OwnerName = model.OwnerName;
                        existingModel.OwnerPhone = model.OwnerPhone;
                        existingModel.Address =  model.Address;
                        existingModel.Remark = model.Remark;
                        model.UpdatedAt = DateTime.Now;

                        returnMsg = "Updated";
                        log.Logs = $"{loginUserName} Modify Partner ({existingModel.OwnerName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                    }
                }
                else
                {
                    model.Type = "1";
                    model.CreatedAt = DateTime.Now;
                    _context.Owner.Add(model);

                    returnMsg = "Created";
                    log.Logs = $"{loginUserName} Add new Partner ({model.OwnerName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }

                #region log area
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "OwnerRelated";

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessOwner", new { message = returnMsg });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOwner(int id)
        {
            var item = await _context.Owner.FindAsync(id);
            if (item == null) return NotFound();

            _context.Owner.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public IActionResult SuccessOwner(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        #endregion


        #region Agent

        public async Task<IActionResult> AgentList()
        {
            //var ls = await _context.Owner.Where(x => x.Type == "2")
            //    .Select(x => new AgentModel
            //    {
            //        AgentName = x.OwnerName,
            //        AgentPhone = x.OwnerPhone,
            //        Address = x.Address,
            //        Remark = x.Remark,
            //        CreatedAt = x.CreatedAt,
            //    }).ToListAsync();

            //await _context.Agent.AddRangeAsync(ls);
            //await _context.SaveChangesAsync();

            //return View(await _context.Agent.AsNoTracking().ToListAsync());

            var model = await _context.Owner.AsNoTracking().Where(x => x.Type == "2")
                .Select(x => new AOViewModel
                {
                    Id = x.Id,
                    OwnerName = x.OwnerName,
                    OwnerPhone = x.OwnerPhone,
                    Address = x.Address,
                    Remark = x.Remark,
                    PropertyCount = x.Properties.Count(),
                }).ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> AgentCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                return View(await _context.Owner.FindAsync(id));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgentCreate(OwnerModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";
                if (model.Id > 0)
                {
                    var existingModel = await _context.Owner.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.OwnerName = model.OwnerName;
                        existingModel.OwnerName = model.OwnerName;
                        existingModel.Address =  model.Address;
                        existingModel.Remark = model.Remark;
                        model.UpdatedAt = DateTime.Now;

                        returnMsg = "Updated";
                        log.Logs = $"{loginUserName} Modify Partner ({existingModel.OwnerName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                    }
                }
                else
                {
                    model.Type = "2";
                    model.CreatedAt = DateTime.Now;
                    _context.Owner.Add(model);

                    returnMsg = "Created";
                    log.Logs = $"{loginUserName} Add new Partner ({model.OwnerName}) @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }

                #region log area
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "AgentRelated";

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessAgent", new { message = returnMsg });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAgent(int id)
        {
            var item = await _context.Agent.FindAsync(id);
            if (item == null) return NotFound();

            _context.Agent.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public IActionResult SuccessAgent(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        #endregion
    }
}

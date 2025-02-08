using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
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
            return View(await _context.Client.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> ClientCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                return View(await _context.Client.FindAsync(id));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClientCreate(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
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
                    }
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    _context.Client.Add(model);

                    returnMsg = "Created";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessClient", new { message = returnMsg });
            }
            return View();
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
            return View(await _context.Owner.AsNoTracking().ToListAsync());
        }
        public async Task<IActionResult> OwnerCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                return View(await _context.Owner.FindAsync(id));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OwnerCreate(OwnerModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
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
                    }
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    _context.Owner.Add(model);

                    returnMsg = "Created";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("SuccessOwner", new { message = returnMsg });
            }
            return View();
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

            return View(await _context.Agent.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> AgentCreate(int? id)
        {
            ViewBag.CreateOrUpdate = (id == null || id <= 0) ? "Create New" : "Update";

            if (id != null && id > 0)
            {
                return View(await _context.Agent.FindAsync(id));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgentCreate(AgentModel model)
        {
            if (ModelState.IsValid)
            {
                string returnMsg = "Created";
                if (model.Id > 0)
                {
                    var existingModel = await _context.Agent.FindAsync(model.Id);
                    if (existingModel != null)
                    {
                        existingModel.AgentName = model.AgentName;
                        existingModel.AgentPhone = model.AgentPhone;
                        existingModel.Address =  model.Address;
                        existingModel.FBPage = model.FBPage;
                        existingModel.Remark = model.Remark;
                        model.UpdatedAt = DateTime.Now;

                        returnMsg = "Updated";
                    }
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    _context.Agent.Add(model);

                    returnMsg = "Created";
                }
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

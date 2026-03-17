using System.Security.Claims;
using CloseFriendMyanamr.Models.ClientManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class ClientRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                ? userId
                : 0;
        }

        public async Task<IActionResult> ClicentRequestList()
        {
            var userId = GetCurrentUserId();
            ViewBag.UserId = userId;

            var query = _context.ClientRequirement
                .AsNoTracking()
                .Include(x => x.Client)
                .AsQueryable();

            if (User.IsInRole("MobileUser"))
            {
                query = query.Where(x => x.ClientId == userId);
            }

            return View(await query
                .OrderByDescending(x => x.Id)
                .ToListAsync());
        }

        public async Task<IActionResult> ClientRequirement(int clientId, int? clrId)
        {
            var currentUserId = GetCurrentUserId();
            if (User.IsInRole("MobileUser"))
            {
                if (currentUserId <= 0)
                {
                    return Challenge();
                }

                clientId = currentUserId;
            }

            #region for select value
            var propertyTypes = await _context.PropertyType.AsNoTracking()
                .Select(x => new { x.Id, x.TypeName })
                .ToListAsync();

            if (propertyTypes == null || !propertyTypes.Any())
            {
                ViewData["PropertyType"] = new SelectList(new List<object>(), "TypeName", "TypeName");
            }
            else
            {
                ViewData["PropertyType"] = new SelectList(propertyTypes, "TypeName", "TypeName");
            }

            var buildingTypes = await _context.BuildingType.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (buildingTypes == null || !buildingTypes.Any())
            {
                ViewData["BuildingType"] = new SelectList(new List<object>(), "Name", "Name");
            }
            else
            {
                ViewData["BuildingType"] = new SelectList(buildingTypes, "Name", "Name"); // Corrected this line
            }

            var townships = await _context.Township.AsNoTracking()
                .Select(x => x.TownshipMM)
                .ToListAsync();


            if (townships == null || !townships.Any())
            {
                ViewData["Townships"] = new List<string>();
            }
            else
            {
                ViewData["Townships"] = townships;
            }

            var facilities = await _context.Facilities.AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync();

            if (facilities == null || !facilities.Any())
            {
                ViewData["Facilities"] = new List<string>();
            }
            else
            {
                ViewData["Facilities"] = facilities;
            }

            #endregion

            #region client info

            var clientInfoObj = await _context.Client.AsNoTracking().Select(x => new { x.ClientName, x.ClientPhone, x.Id }).FirstOrDefaultAsync(x => x.Id == clientId);
            ViewBag.ClientInfo = clientInfoObj != null ? (clientInfoObj.ClientName + " [" + clientInfoObj.ClientPhone + "]") : "";
            ViewBag.ClientId = clientId;

            #endregion

            ClientRequirementModel model;
            if (clrId.HasValue) // Check if it's an update
            {
                model = await _context.ClientRequirement.AsNoTracking().FirstOrDefaultAsync(x => x.Id == clrId.Value);
                if (model == null)
                {
                    return NotFound();
                }

                if (User.IsInRole("MobileUser") && model.ClientId != currentUserId)
                {
                    return Forbid();
                }

                clientId = model.ClientId;
            }
            else
            {
                model = new ClientRequirementModel { ClientId = clientId }; // Set ClientId for new record
            }


            // Pre-select townships and facilities for update
            if (model.Township != null)
            {
                ViewBag.SelectedTownships = model.Township.Split(',');
            }
            if (model.Facilities != null)
            {
                ViewBag.SelectedFacilities = model.Facilities.Split(',');
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientRequirementCreate(ClientRequirementModel model, List<string> selectedTownships, List<string> selectedFacilities)
        {
            var currentUserId = GetCurrentUserId();
            if (User.IsInRole("MobileUser"))
            {
                if (currentUserId <= 0)
                {
                    return Challenge();
                }

                model.ClientId = currentUserId;
            }

            if (ModelState.IsValid)
            {
                model.Township = string.Join(",", selectedTownships ?? new List<string>());
                model.Facilities = string.Join(",", selectedFacilities ?? new List<string>());

                if (model.Id == 0) // Check if it's a new record or update
                {
                    _context.ClientRequirement.Add(model);
                }
                else
                {
                    var existingRequirement = await _context.ClientRequirement.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (existingRequirement == null)
                    {
                        return NotFound();
                    }

                    if (User.IsInRole("MobileUser") && existingRequirement.ClientId != currentUserId)
                    {
                        return Forbid();
                    }

                    existingRequirement.ClientId = User.IsInRole("MobileUser") ? currentUserId : model.ClientId;
                    existingRequirement.Purpose = model.Purpose;
                    existingRequirement.PropertyType = model.PropertyType;
                    existingRequirement.BuildingCondition = model.BuildingCondition;
                    existingRequirement.BuildingType = model.BuildingType;
                    existingRequirement.Street = model.Street;
                    existingRequirement.Ward = model.Ward;
                    existingRequirement.CondoName = model.CondoName;
                    existingRequirement.StartPrice = model.StartPrice;
                    existingRequirement.EndPrice = model.EndPrice;
                    existingRequirement.Area = model.Area;
                    existingRequirement.MasterBed = model.MasterBed;
                    existingRequirement.FloorMin = model.FloorMin;
                    existingRequirement.FloorMax = model.FloorMax;
                    existingRequirement.SpecialRequest = model.SpecialRequest;
                    existingRequirement.Township = model.Township;
                    existingRequirement.Facilities = model.Facilities;
                    existingRequirement.Status = model.Status;
                    existingRequirement.RequestDate = model.RequestDate;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("ClicentRequestList");
            }

            #region for select value
            var propertyTypes = await _context.PropertyType.AsNoTracking()
                           .Select(x => new { x.Id, x.TypeName })
                           .ToListAsync();

            if (propertyTypes == null || !propertyTypes.Any())
            {
                ViewData["PropertyType"] = new SelectList(new List<object>(), "TypeName", "TypeName");
            }
            else
            {
                ViewData["PropertyType"] = new SelectList(propertyTypes, "TypeName", "TypeName");
            }

            var buildingTypes = await _context.BuildingType.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (buildingTypes == null || !buildingTypes.Any())
            {
                ViewData["BuildingType"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["BuildingType"] = new SelectList(buildingTypes, "Id", "Name"); // Corrected this line
            }

            var townships = await _context.Township.AsNoTracking()
                .Select(x => x.TownshipMM)
                .ToListAsync();


            if (townships == null || !townships.Any())
            {
                ViewData["Townships"] = new List<string>();
            }
            else
            {
                ViewData["Townships"] = townships;
            }

            var facilities = await _context.Facilities.AsNoTracking()
                .Select(x => x.Name)
                .ToListAsync();

            if (facilities == null || !facilities.Any())
            {
                ViewData["Facilities"] = new List<string>();
            }
            else
            {
                ViewData["Facilities"] = facilities;
            }
            #endregion

            #region client info

            var clientInfoObj = await _context.Client.AsNoTracking().Select(x => new { x.ClientName, x.ClientPhone, x.Id }).FirstOrDefaultAsync(x => x.Id == model.ClientId);
            ViewBag.ClientInfo = clientInfoObj != null ? (clientInfoObj.ClientName + " [" + clientInfoObj.ClientPhone + "]") : "";
            ViewBag.ClientId = model.ClientId;

            #endregion

            ViewBag.SelectedTownships = selectedTownships;
            ViewBag.SelectedFacilities = selectedFacilities;

            return View(model);
        }

        //Delete Action
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            var clientRequirement = await _context.ClientRequirement.FindAsync(id);
            if (clientRequirement == null)
            {
                return NotFound();
            }

            if (User.IsInRole("MobileUser") && clientRequirement.ClientId != currentUserId)
            {
                return Forbid();
            }

            _context.ClientRequirement.Remove(clientRequirement);
            await _context.SaveChangesAsync();
            return RedirectToAction("ClicentRequestList"); // Or wherever you want to redirect
        }
    }
}

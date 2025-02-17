using CloseFriendMyanamr.Models.ClientManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class ClientRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientRequestController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> ClicentRequestList()
        {
            return View(await _context.ClientRequirement.AsNoTracking().Include(x => x.Client).ToListAsync());
        }

        public async Task<IActionResult> ClientRequirement(int clientId)
        {
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
            ViewBag.ClientInfo = clientInfoObj != null? (clientInfoObj.ClientName + " [" + clientInfoObj.ClientPhone + "]") : "";
            ViewBag.ClientId = clientId;

            #endregion

            return View(new ClientRequirementModel());
        }

        [HttpPost]
        public async Task<IActionResult> ClientRequirementCreate(ClientRequirementModel model, List<string> selectedTownships, List<string> selectedFacilities)
        {
            if (ModelState.IsValid)
            {
                model.Township = string.Join(",", selectedTownships ?? new List<string>());
                model.Facilities = string.Join(",", selectedFacilities ?? new List<string>());

                _context.ClientRequirement.Add(model);
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

            return View(model);
        }
    }
}

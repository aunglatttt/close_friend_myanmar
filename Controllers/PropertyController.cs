using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.Property;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SimpleDataWebsite.Data;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace CloseFriendMyanamr.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public PropertyController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context=context;
            _env=env;
        }

        #region save property
        public async Task<IActionResult> NewProperty(int? id)
        {
            #region for select value

            #region owner
            var owners = await _context.Owner.AsNoTracking()
                .Select(x => new { x.Id, x.OwnerName })
                .ToListAsync();

            if (owners == null || !owners.Any())
            {
                ViewData["OwnerList"] = new SelectList(new List<object>(), "Id", "OwnerName");
            }
            else
            {
                ViewData["OwnerList"] = new SelectList(owners, "Id", "OwnerName");
            }
            #endregion

            #region property type
            var propertyTypes = await _context.PropertyType.AsNoTracking()
                .Select(x => new { x.ShortCode, x.TypeName })
                .ToListAsync();

            if (propertyTypes == null || !propertyTypes.Any())
            {
                ViewData["PropertyTypeList"] = new SelectList(new List<object>(), "ShortCode", "TypeName");
            }
            else
            {
                ViewData["PropertyTypeList"] = new SelectList(propertyTypes, "ShortCode", "TypeName");
            }
            #endregion

            #region building type
            var buildingTypes = await _context.BuildingType.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (buildingTypes == null || !buildingTypes.Any())
            {
                ViewData["BuildingTypeList"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["BuildingTypeList"] = new SelectList(buildingTypes, "Id", "Name");
            }
            #endregion

            #region townships
            var townships = await _context.Township.AsNoTracking()
                .Select(x => new { x.Township, x.TownshipMM })
                .ToListAsync();

            if (townships == null || !townships.Any())
            {
                ViewData["TownshipList"] = new SelectList(new List<object>(), "Township", "TownshipMM");
            }
            else
            {
                ViewData["TownshipList"] = new SelectList(townships, "Township", "TownshipMM");
            }
            #endregion


            #region facilities
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

            #endregion

            if (id > 0)
            {
                var model = await _context.Property.AsNoTracking().Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == id);
                if (model != null)
                {
                    if (model.Facilities != null)
                    {
                        ViewBag.SelectedFacilities = model.Facilities.Split(',');
                    }
                    model.PurposeSale = model.Purpose == "Sale";
                    model.PurposeRent = model.Purpose == "Rent";
                }
                return View(model);
            }
            return View(new PropertyModel());
        }

        [HttpPost]
        public async Task<IActionResult> NewProperty(PropertyModel model, List<string> selectedFacilities)
        {
            if (ModelState.IsValid)
            {
                // Check if the user selected "အသစ်ထည့်ရန်" (Add New Owner)
                if (model.OwnerId == 0)
                {

                    if (string.IsNullOrEmpty(model.OwnerName) || string.IsNullOrEmpty(model.OwnerPhone) || string.IsNullOrEmpty(model.OwnerTypeSelect))
                    {
                        #region for select value

                        #region owner
                        var owners = await _context.Owner.AsNoTracking()
                            .Select(x => new { x.Id, x.OwnerName })
                            .ToListAsync();

                        if (owners == null || !owners.Any())
                        {
                            ViewData["OwnerList"] = new SelectList(new List<object>(), "Id", "OwnerName");
                        }
                        else
                        {
                            ViewData["OwnerList"] = new SelectList(owners, "Id", "OwnerName");
                        }
                        #endregion

                        #region property type
                        var propertyTypes = await _context.PropertyType.AsNoTracking()
                            .Select(x => new { x.ShortCode, x.TypeName })
                            .ToListAsync();

                        if (propertyTypes == null || !propertyTypes.Any())
                        {
                            ViewData["PropertyTypeList"] = new SelectList(new List<object>(), "ShortCode", "TypeName");
                        }
                        else
                        {
                            ViewData["PropertyTypeList"] = new SelectList(propertyTypes, "ShortCode", "TypeName");
                        }
                        #endregion

                        #region building type
                        var buildingTypes = await _context.BuildingType.AsNoTracking()
                            .Select(x => new { x.Id, x.Name })
                            .ToListAsync();

                        if (buildingTypes == null || !buildingTypes.Any())
                        {
                            ViewData["BuildingTypeList"] = new SelectList(new List<object>(), "Id", "Name");
                        }
                        else
                        {
                            ViewData["BuildingTypeList"] = new SelectList(buildingTypes, "Id", "Name");
                        }
                        #endregion

                        #region townships
                        var townships = await _context.Township.AsNoTracking()
                            .Select(x => new { x.Township, x.TownshipMM })
                            .ToListAsync();

                        if (townships == null || !townships.Any())
                        {
                            ViewData["TownshipList"] = new SelectList(new List<object>(), "Township", "TownshipMM");
                        }
                        else
                        {
                            ViewData["TownshipList"] = new SelectList(townships, "Township", "TownshipMM");
                        }
                        #endregion


                        #region facilities
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

                        #endregion


                        ViewBag.Error = "Owner information is required when adding a new owner.";
                        return View(model);
                    }

                    model.Owner = new OwnerModel
                    {
                        OwnerName = model.OwnerName,
                        OwnerPhone = model.OwnerPhone,
                        Type = model.OwnerTypeSelect,
                        Address = model.OwnerAddress,
                        CreatedAt = DateTime.Now
                    };
                }

                model.Facilities = string.Join(",", selectedFacilities ?? new List<string>());
                model.Purpose = model.PurposeSale == true ? "Sale" : "Rent";

                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";
                if (model.Id == 0)
                {
                    if (string.IsNullOrEmpty(model.Code))
                    {
                        Random random = new Random();
                        model.Code = model.PropertyType + random.Next(100000, 999999).ToString();
                    }


                    _context.Property.Add(model);

                    log.Logs = $"{loginUserName} Add new Property {model.Code} @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }
                else
                {
                    _context.Property.Update(model);
                    log.Logs = $"{loginUserName} Modify Property {model.Code} @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }

                #region log area
                log.EmployeeId = int.Parse(userId ?? "0");
                log.LogsDate = DateTime.Now;
                log.Type = "PropertyRelated";

                _context.Log.Add(log);
                #endregion

                await _context.SaveChangesAsync();

                return RedirectToAction("SearchProperty");
            }
            else
            {
                #region for select value

                #region owner
                var owners = await _context.Owner.AsNoTracking()
                    .Select(x => new { x.Id, x.OwnerName })
                    .ToListAsync();

                if (owners == null || !owners.Any())
                {
                    ViewData["OwnerList"] = new SelectList(new List<object>(), "Id", "OwnerName");
                }
                else
                {
                    ViewData["OwnerList"] = new SelectList(owners, "Id", "OwnerName");
                }
                #endregion

                #region property type
                var propertyTypes = await _context.PropertyType.AsNoTracking()
                    .Select(x => new { x.ShortCode, x.TypeName })
                    .ToListAsync();

                if (propertyTypes == null || !propertyTypes.Any())
                {
                    ViewData["PropertyTypeList"] = new SelectList(new List<object>(), "ShortCode", "TypeName");
                }
                else
                {
                    ViewData["PropertyTypeList"] = new SelectList(propertyTypes, "ShortCode", "TypeName");
                }
                #endregion

                #region building type
                var buildingTypes = await _context.BuildingType.AsNoTracking()
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

                if (buildingTypes == null || !buildingTypes.Any())
                {
                    ViewData["BuildingTypeList"] = new SelectList(new List<object>(), "Id", "Name");
                }
                else
                {
                    ViewData["BuildingTypeList"] = new SelectList(buildingTypes, "Id", "Name");
                }
                #endregion

                #region townships
                var townships = await _context.Township.AsNoTracking()
                    .Select(x => new { x.Township, x.TownshipMM })
                    .ToListAsync();

                if (townships == null || !townships.Any())
                {
                    ViewData["TownshipList"] = new SelectList(new List<object>(), "Township", "TownshipMM");
                }
                else
                {
                    ViewData["TownshipList"] = new SelectList(townships, "Township", "TownshipMM");
                }
                #endregion


                #region facilities
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

                #endregion

                return View(model);
            }

        }
        #endregion
        
        #region search property
        public async Task<IActionResult> SearchProperty(int startIndex = 1, int showCount = 10)
        {
            #region for select value

            #region owner
            var owners = await _context.Owner.AsNoTracking()
                .Select(x => new { x.Id, x.OwnerName })
                .ToListAsync();

            if (owners == null || !owners.Any())
            {
                ViewData["OwnerList"] = new SelectList(new List<object>(), "OwnerName", "OwnerName");
            }
            else
            {
                ViewData["OwnerList"] = new SelectList(owners, "OwnerName", "OwnerName");
            }
            #endregion

            #region property type
            var propertyTypes = await _context.PropertyType.AsNoTracking()
                .Select(x => new { x.ShortCode, x.TypeName })
                .ToListAsync();

            if (propertyTypes == null || !propertyTypes.Any())
            {
                ViewData["PropertyTypeList"] = new SelectList(new List<object>(), "ShortCode", "TypeName");
            }
            else
            {
                ViewData["PropertyTypeList"] = new SelectList(propertyTypes, "ShortCode", "TypeName");
            }
            #endregion

            #region building type
            var buildingTypes = await _context.BuildingType.AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            if (buildingTypes == null || !buildingTypes.Any())
            {
                ViewData["BuildingTypeList"] = new SelectList(new List<object>(), "Id", "Name");
            }
            else
            {
                ViewData["BuildingTypeList"] = new SelectList(buildingTypes, "Id", "Name");
            }
            #endregion

            #region townships
            var townships = await _context.Township.AsNoTracking()
                .Select(x => new { x.Township, x.TownshipMM })
                .ToListAsync();

            if (townships == null || !townships.Any())
            {
                ViewData["TownshipList"] = new SelectList(new List<object>(), "Township", "TownshipMM");
            }
            else
            {
                ViewData["TownshipList"] = new SelectList(townships, "Township", "TownshipMM");
            }
            #endregion

            #region condo name

            var propertyObj = await _context.Property.AsNoTracking()
                .Select(x => new { x.CondoName, x.Id, x.Size, x.Area })
                //.Where(x => !string.IsNullOrEmpty(x.CondoName))
                //.Distinct()
                .ToListAsync();

            var condoNames = propertyObj.Select(x => new { x.CondoName, x.Id }).Where(x => !string.IsNullOrEmpty(x.CondoName)).Distinct().ToList();
            if (condoNames == null || !condoNames.Any())
            {
                ViewData["CondoNameList"] = new SelectList(new List<object>(), "CondoName", "CondoName");
            }
            else
            {
                ViewData["CondoNameList"] = new SelectList(condoNames, "CondoName", "CondoName");
            }


            var sizeOptoins = propertyObj.Select(x => new { x.Size, x.Id }).Where(x => !string.IsNullOrEmpty(x.Size)).Distinct().ToList();
            if (sizeOptoins == null || !sizeOptoins.Any())
            {
                ViewData["SizeList"] = new SelectList(new List<object>(), "Size", "Size");
            }
            else
            {
                ViewData["SizeList"] = new SelectList(sizeOptoins, "Size", "Size");
            }

            var areaOptoins = propertyObj.Select(x => new { x.Area, x.Id }).Where(x => !string.IsNullOrEmpty(x.Area)).Distinct().ToList();
            if (areaOptoins == null || !areaOptoins.Any())
            {
                ViewData["AreaList"] = new SelectList(new List<object>(), "Area", "Area");
            }
            else
            {
                ViewData["AreaList"] = new SelectList(areaOptoins, "Area", "Area");
            }

            #endregion

            #region facilities
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



            #endregion

            return View(new PaginationViewModelForProperty());
        }

        public async Task<IActionResult> GetProperties(
                 int page = 1,
                int pageSize = 10,
                string propertyType = null,
                string buildingType = null,
                string ownerAgent = null,
                string purpose = null,
                int priceFrom = 0,
                int priceTo = 0,
                string condoName = null,
                string face = null,
                string buildingNo = null,
                string street = null,
                string ward = null,
                string commentInfo = null,
                string ownership = null,
                string salerOwnType = null,
                int floor = 0,
                string size = null,
                string area = null,
                int masterRoom = 0,
                int sigleRoom = 0,
                string status = null,
                string postStatus = null,
                List<string> townships = null,
                List<string> facilities = null)
        {

            try
            {

                var query = await _context.Property.AsNoTracking()
                     .Where(x => x.Status != "Delete" &&
                         (string.IsNullOrEmpty(propertyType) || x.PropertyType == propertyType) &&
                         (string.IsNullOrEmpty(buildingType) || x.BuildingType == buildingType) &&
                         (string.IsNullOrEmpty(ownerAgent) || x.Owner.OwnerName == ownerAgent) &&
                         (string.IsNullOrEmpty(purpose) || x.Purpose == purpose) &&
                         (priceFrom <= 0 || (x.SalePrice >= priceFrom || x.RentPrice >= priceFrom)) &&
                         (priceTo <= 0 || (x.SalePrice <= priceTo || x.RentPrice <= priceTo)) &&
                         (string.IsNullOrEmpty(condoName) || x.CondoName == condoName) &&
                         (string.IsNullOrEmpty(face) || x.Face == face) &&
                         (string.IsNullOrEmpty(buildingNo) || x.Building == buildingNo) &&
                         (string.IsNullOrEmpty(street) || EF.Functions.Like(x.Street.ToLower(), $"%{street.ToLower()}%")) &&
                         (string.IsNullOrEmpty(ward) || EF.Functions.Like(x.Ward.ToLower(), $"%{ward.ToLower()}%")) &&
                         (string.IsNullOrEmpty(commentInfo) || EF.Functions.Like(x.Comment.ToLower(), $"%{commentInfo.ToLower()}%")) &&
                         (string.IsNullOrEmpty(ownership) || x.Township == ownership) &&
                         (string.IsNullOrEmpty(salerOwnType) || x.SalerOwnerType == salerOwnType) &&
                         (floor <= 0 || x.Floor == floor) &&
                         (string.IsNullOrEmpty(size) || x.Size == size) &&
                         (string.IsNullOrEmpty(area) || x.Area == area) &&
                         (masterRoom <= 0 || x.MasterBed == masterRoom) &&
                         (sigleRoom <= 0 || x.SingleBed == sigleRoom) &&
                         (string.IsNullOrEmpty(status) || x.Status == status) &&
                         (string.IsNullOrEmpty(postStatus) || x.PostStatus == postStatus) &&
                         (townships == null || townships.Contains(x.Township)) &&
                         (facilities == null || facilities.Any(f => EF.Functions.Like(x.Facilities.ToLower(), "%" + f.ToLower() + "%"))))
                     .Include(x => x.Owner)
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .Select(x => new PropertyViewModel
                     {
                         Id = x.Id,
                         Code = x.Code,
                         AvaliableDate = x.AvailableDate.ToString("MMM dd, yyyy"),
                         Status = x.Status,
                         Township = x.Township,
                         Street = x.Street,
                         Comment = x.Comment,
                         Room = x.Room,
                         Price = x.Purpose == "Sale" ? x.SalePrice : x.RentPrice,
                         Owner = x.Owner != null ? (x.Owner.OwnerName + "(" + x.Owner.OwnerPhone) + ")" : "",
                         Remark = x.Remark
                     })
                     .ToListAsync();


                var count = await _context.Property.AsNoTracking()
                       .Where(x => x.Status != "Delete" &&
                         (string.IsNullOrEmpty(propertyType) || x.PropertyType == propertyType) &&
                         (string.IsNullOrEmpty(buildingType) || x.BuildingType == buildingType) &&
                         (string.IsNullOrEmpty(ownerAgent) || x.Owner.OwnerName == ownerAgent) &&
                         (string.IsNullOrEmpty(purpose) || x.Purpose == purpose) &&
                         (priceFrom <= 0 || (x.SalePrice >= priceFrom || x.RentPrice >= priceFrom)) &&
                         (priceTo <= 0 || (x.SalePrice <= priceTo || x.RentPrice <= priceTo)) &&
                         (string.IsNullOrEmpty(condoName) || x.CondoName == condoName) &&
                         (string.IsNullOrEmpty(face) || x.Face == face) &&
                         (string.IsNullOrEmpty(buildingNo) || x.Building == buildingNo) &&
                         (string.IsNullOrEmpty(street) || EF.Functions.Like(x.Street.ToLower(), $"%{street.ToLower()}%")) &&
                         (string.IsNullOrEmpty(ward) || EF.Functions.Like(x.Ward.ToLower(), $"%{ward.ToLower()}%")) &&
                         (string.IsNullOrEmpty(commentInfo) || EF.Functions.Like(x.Comment.ToLower(), $"%{commentInfo.ToLower()}%")) &&
                         (string.IsNullOrEmpty(ownership) || x.Township == ownership) &&
                         (string.IsNullOrEmpty(salerOwnType) || x.SalerOwnerType == salerOwnType) &&
                         (floor <= 0 || x.Floor == floor) &&
                         (string.IsNullOrEmpty(size) || x.Size == size) &&
                         (string.IsNullOrEmpty(area) || x.Area == area) &&
                         (masterRoom <= 0 || x.MasterBed == masterRoom) &&
                         (sigleRoom <= 0 || x.SingleBed == sigleRoom) &&
                         (string.IsNullOrEmpty(status) || x.Status == status) &&
                         (string.IsNullOrEmpty(postStatus) || x.PostStatus == postStatus) &&
                         (townships == null || townships.Contains(x.Township)) &&
                         (facilities == null || facilities.Any(f => EF.Functions.Like(x.Facilities.ToLower(), "%" + f.ToLower() + "%"))))
                     .Include(x => x.Owner)
                     .CountAsync();


                var filters = new PaginationViewModelForProperty
                {
                    Propertys = query,
                    PropertyType = propertyType,
                    BuildingType = buildingType,
                    OwnerAgent = ownerAgent,
                    Purpose = purpose,
                    PriceFrom = priceFrom,
                    PriceTo = priceTo,
                    CondoName = condoName,
                    Face = face,
                    BuildingNo = buildingNo,
                    Street = street,
                    Ward = ward,
                    CommentInfo = commentInfo,
                    Ownership = ownership,
                    SalerOwnType = salerOwnType,
                    Floor = floor,
                    Size = size,
                    Area = area,
                    MasterRoom = masterRoom,
                    SigleRoom = sigleRoom,
                    Status = status,
                    PostStatus = postStatus,
                    Townships = townships, // Include townships
                    Facilities = facilities // Include facilities
                };


                return PartialView("_PropertyPartial", filters);
            }
            catch (Exception ex)
            {
                return PartialView("_PropertyPartial");
            }
        }

        #endregion
       
        public async Task<IActionResult> PropertyList()
        {
            var model = await _context.Property.AsNoTracking()
                .Select(x => new PropertyViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Status = x.Status,
                    AvaliableDate = x.AvailableDate.ToString("MMM dd, yyyy"),
                    LastCheckedDate = x.LastCheckedDate.ToString("MMM dd, yyyy"),
                    Street = x.Street,
                    Comment = x.Comment,
                    Price = x.Purpose == "Sale" ? x.SalePrice : x.RentPrice,
                    Owner = x.Owner != null ? (x.Owner.OwnerName + "(" + x.Owner.OwnerPhone) + ")" : "",
                    Remark = x.Remark
                })
                .ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> PropertyInfo(int propertyId)
        {
            var model = await _context.Property.AsNoTracking().Include(x => x.Photos).Include(x => x.Owner).Where(x => x.Id == propertyId).FirstOrDefaultAsync();

            if(model != null)
            {
                switch (model.SaleCommission)
                {
                    case 1:
                        model.SaleCommissionString = "2% (အပြည့်အဝ)";
                        break;
                    case 2:
                        model.SaleCommissionString = "1% (တစ်ဝက်)";
                        break;
                    case 3:
                        model.SaleCommissionString = "1/3 (သုံးပုံတစ်ပုံ)";
                        break;
                    case 4:
                        model.SaleCommissionString = "အကျိုးတူ (အညီမျှ)";
                        break;
                    case 5:
                        model.SaleCommissionString = "အဆင်ပြေသလို ညှိယူရန်";
                        break;
                    default:
                        model.SaleCommissionString = "";
                        break;
                }

                switch (model.RentCommision)
                {
                    case 1:
                        model.RentCommisionString = "၁လစာ (အိမ်ရှင်+အိမ်ငှား)";
                        break;
                    case 2:
                        model.RentCommisionString = "၁လစာ (တစ်ခြမ်း)";
                        break;
                    case 3:
                        model.RentCommisionString = "1/3 (သုံးပုံတစ်ပုံ)";
                        break;
                    case 4:
                        model.RentCommisionString = "အကျိုးတူ (အညီမျှ)";
                        break;
                    case 5:
                        model.RentCommisionString = "အဆင်ပြေသလို ညှိယူရန်";
                        break;
                    default:
                        model.RentCommisionString = "";
                        break;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Gallery(int propertyId)
        {
            ViewBag.PropertyId = propertyId;
            return View(await _context.Photo.AsNoTracking().Where(x => x.PropertyId == propertyId).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PropertySearchByCode(string code)
        {
            if(!string.IsNullOrEmpty(code))
            {
                int propertyId = await _context.Property
                    .AsNoTracking()
                    .Where(x => x.Code.ToLower() == code.ToLower())
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                if (propertyId >0)
                    return RedirectToAction("NewProperty", new { id = propertyId });
                else
                    return View();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, int propertyId, int TempId)
        {
            foreach (var file in files)
            {
                if (file.Length > 0 && IsImage(file))
                {
                    string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{file.FileName}";
                    var filePath = Path.Combine(_env.WebRootPath, "PropertyPhoto", uniqueFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var image = new PhotoModel
                    {
                        PropertyId = propertyId,
                        Title = file.FileName,
                        Location = uniqueFileName,
                        //purposeid = purposeId, // Save PurposeId to the database
                        //TempId = TempId // Save TempId to the database
                    };

                    _context.Photo.Add(image);
                }
            }
            if(files.Count > 0)
                await _context.SaveChangesAsync();
            return RedirectToAction("Gallery", new { propertyId = propertyId });
        }

        private bool IsImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int propertyId)
        {
            var image = await _context.Photo.FindAsync(id);
            if (image != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, "PropertyPhoto", image.Location);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Photo.Remove(image);
                await _context.SaveChangesAsync();
            }
            ViewBag.PropertyId = propertyId;

            return RedirectToAction("Gallery", new { propertyId = propertyId });
        }
    }
}

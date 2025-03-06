using CloseFriendMyanamr.Helper;
using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.Property;
using CloseFriendMyanamr.Models.UserManagement;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimpleDataWebsite.Data;
using System.Drawing;
using System.Security.Claims;

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
                .OrderBy(x => x.OwnerName)
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
                .OrderBy(x => x.TownshipMM)
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
                var model = await _context.Property.AsNoTracking().Include(x => x.PropertyFacilities).Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == id);
                if (model != null)
                {
                    //if (model.Facilities != null)
                    //{
                    //ViewBag.SelectedFacilities = "aung, bb, adfdsf, ss".Split(',');
                    //}
                    model.OwnerName = model.Owner?.OwnerName;
                    model.OwnerPhone = model.Owner?.OwnerPhone;
                    model.OwnerTypeSelect = model.Owner?.Type??"";
                    model.OwnerAddress = model.Owner?.Address;
                    if (model.PropertyFacilities.Any())
                    {
                        ViewBag.SelectedFacilities = model.PropertyFacilities.Select(x => x.Facility).ToList();
                    }
                    else
                    {
                        ViewBag.SelectedFacilities = new List<string>();
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
            bool isCodeOk = true;
            if (!string.IsNullOrEmpty(model.Code) && model.Id <= 0)
            {
                bool isFound = await _context.Property.AsNoTracking().AnyAsync(x => x.Code == model.Code);
                isCodeOk = isFound == true ? false : true;
            }

            // Check for duplicate HouseNo, Street, CondoName, Floor, RoomNo (for both create and update)
            bool isDuplicate = await _context.Property.AsNoTracking()
                .AnyAsync(x => (x.Ward == model.Ward) &&
                            (x.Street == model.Street) &&
                            (x.CondoName == model.CondoName) &&
                            (x.Floor == model.Floor) &&
                            (x.Room == model.Room) &&
                            (x.Owner.OwnerName == model.OwnerName) &&
                            (x.Owner.OwnerPhone == model.OwnerPhone) &&
                            (x.PropertyType == model.PropertyType) &&
                             x.Id != model.Id);


            if (ModelState.IsValid && isCodeOk && !isDuplicate)
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
                            .OrderBy(x => x.OwnerName)
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
                            .OrderBy(x => x.TownshipMM)
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

                //model.Facilities = string.Join(",", selectedFacilities ?? new List<string>());
                var facilites = new List<PropertyFacilityModel>();

                if(selectedFacilities != null && selectedFacilities.Any())
                {
                    foreach(var item in selectedFacilities)
                    {
                        facilites.Add(new PropertyFacilityModel
                        {
                            Facility = item
                        });
                    }
                }
                model.Purpose = model.PurposeSale == true ? "Sale" : "Rent";

                var log = new LogModel();
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                string loginUserName = await _context.Employee.AsNoTracking().Where(x => x.Id == int.Parse(userId??"0")).Select(x => x.EmployeeName).FirstOrDefaultAsync()??"";

                model.LastCheckedById = int.Parse(userId??"0");
                model.LastCheckedDate = DateTime.Now;

                if (string.IsNullOrEmpty(model.Code))
                {
                    //Random random = new Random();
                    //model.Code = model.PropertyType + random.Next(100000, 999999).ToString();
                    int maxNumber = await GetMaxCodeNumberAsync(model.PropertyType); // Get the maximum number for the prefix

                    model.Code = $"{model.PropertyType}{maxNumber + 1}"; // Generate the new Code
                }

                if (model.Id == 0)
                {

                    model.PropertyFacilities = facilites;

                    _context.Property.Add(model);

                    log.Logs = $"{loginUserName} Add new Property {model.Code} @ {DateTime.Now.ToString("MMM dd, yyyy")}";
                }
                else
                {
                    var oldfacilites = await _context.PropertyFacilities.Where(x => x.PropertyId == model.Id).ToListAsync();
                    _context.PropertyFacilities.RemoveRange(oldfacilites);

                    model.PropertyFacilities = facilites;

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

                return RedirectToAction("PropertyList");
            }
            else
            {
                #region for select value

                #region owner
                var owners = await _context.Owner.AsNoTracking()
                    .Select(x => new { x.Id, x.OwnerName })
                    .OrderBy(x => x.OwnerName)
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
                    .OrderBy(x => x.TownshipMM)
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

                if (!isCodeOk)
                {
                    ViewBag.Error = $"Code ({model.Code}) is already exist.";
                }

                if (isDuplicate)
                {
                    ViewBag.Error = "property already exists.";
                }

                #endregion

                return View(model);
            }

        }
        #endregion

        private async Task<int> GetMaxCodeNumberAsync(string prefix)
        {
            var maxCode = await _context.Property
                .Where(x => x.Code.StartsWith(prefix))
                .Select(x => x.Code)
                .ToListAsync(); // Load the data into memory

            // Extract the numeric part after the prefix and sort the codes
            var maxCodeWithNumbers = maxCode
                .Select(code =>
                {
                    var numericPart = code.Substring(prefix.Length);
                    return new
                    {
                        Code = code,
                        Number = int.TryParse(numericPart, out int num) ? num : 0
                    };
                })
                .OrderByDescending(x => x.Number)  // Sort by numeric part in descending order
                .ThenByDescending(x => x.Code)    // Fallback to sort by full code if needed
                .FirstOrDefault();                // Get the first result

            if (maxCodeWithNumbers == null)
            {
                return 0; // No existing code with this prefix, start from 1
            }

            // Return the numeric part of the found code (which will be the highest)
            return maxCodeWithNumbers.Number;
        }



        #region search property
        public async Task<IActionResult> SearchProperty(int startIndex = 1, int showCount = 10)
        {
            #region for select value

            #region owner
            var owners = await _context.Owner.AsNoTracking()
                .Select(x => new { x.Id, x.OwnerName })
                .OrderBy(x => x.OwnerName)
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
                .OrderBy(x => x.TownshipMM)
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

            var areaOptoins = propertyObj.Select(x => new { x.Area, x.Id }).Where(x => x.Area != null).Distinct().ToList();
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
                var query = _context.Property.AsNoTracking()
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
                        (string.IsNullOrEmpty(salerOwnType) || x.SalerOwnType ==  int.Parse(salerOwnType)) &&
                        (floor <= 0 || x.Floor == floor) &&
                        (string.IsNullOrEmpty(size) || x.Size == size) &&
                        (string.IsNullOrEmpty(area) || x.Area == int.Parse(area)) &&
                        (masterRoom <= 0 || x.MasterBed == masterRoom) &&
                        (sigleRoom <= 0 || x.SingleBed == sigleRoom) &&
                        (string.IsNullOrEmpty(status) || x.Status == status) &&
                        (string.IsNullOrEmpty(postStatus) || x.PostStatus == postStatus) &&
                        (townships == null || townships.Contains(x.Township)) &&
                        (facilities == null || x.PropertyFacilities.Any(d => facilities.Contains(d.Facility))))
                    .Include(x => x.Owner)
                    .Include(x => x.PropertyFacilities);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var properties = await query
                    //.Skip((page - 1) * pageSize)
                    //.Take(pageSize)
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

                var filters = new PaginationViewModelForProperty
                {
                    Propertys = properties,
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
                    Townships = townships,
                    Facilities = facilities,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    PageSize = pageSize
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
                //.Include(x => x.Owner)
                //.Include(x => x.LastCheckedBy)
                .Select(x => new PropertyViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Status = x.Status??"",
                    AvaliableDate = x.AvailableDate.ToString("MMM dd, yyyy"),
                    LastCheckedDate = x.LastCheckedDate.ToString("MMM dd, yyyy"),
                    Street = x.Street,
                    Comment = x.Comment,
                    Price = x.Purpose == "Sale" ? x.SalePrice : x.RentPrice,
                    Owner = x.Owner != null ? (x.Owner.OwnerName + "(" + x.Owner.OwnerPhone) + ")" : "",
                    Remark = x.Remark,
                    LastCheckedBy = x.LastCheckedBy != null? x.LastCheckedBy.EmployeeName : ""
                })
                .Where(x => x.Status != "Delete")
                .OrderByDescending(d => d.Id)
                .ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> DeletedPropertyList()
        {
            var model = await _context.Property.AsNoTracking()
                .Select(x => new PropertyViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Status = x.Status??"",
                    AvaliableDate = x.AvailableDate.ToString("MMM dd, yyyy"),
                    LastCheckedDate = x.LastCheckedDate.ToString("MMM dd, yyyy"),
                    LastCheckedBy = x.LastCheckedBy != null? x.LastCheckedBy.EmployeeName : "",
                    Street = x.Street,
                    Comment = x.Comment,
                    Price = x.Purpose == "Sale" ? x.SalePrice : x.RentPrice,
                    Owner = x.Owner != null ? (x.Owner.OwnerName + "(" + x.Owner.OwnerPhone) + ")" : "",
                    Remark = x.Remark
                })
                .Where(x => x.Status == "Delete")
                .ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> PropertyInfo(int propertyId)
        {
            var model = await _context.Property.AsNoTracking().Include(x => x.Photos).Include(x => x.PropertyFacilities).Include(x => x.Owner).Where(x => x.Id == propertyId).FirstOrDefaultAsync();

            if(model != null)
            {
                model.PropertyType = await _context.PropertyType.AsNoTracking().Where(x => x.ShortCode == model.PropertyType).Select(x => x.TypeName).FirstOrDefaultAsync()??model.PropertyType;
                
                if(model.BuildingType != null)
                {
                    int buildingTypeId = int.TryParse(model.BuildingType, out int result) ? result : 0;
                    model.BuildingType = await _context.BuildingType.AsNoTracking().Where(x => x.Id == buildingTypeId).Select(x => x.Name).FirstOrDefaultAsync()??model.BuildingType;
                }

                #region ownership
               
                switch (model.SalerOwnType)
                {
                    case 1:
                        model.SalerOwnTypeString = "အမည်ပေါက်";
                        break;
                    case 2:
                        model.SalerOwnTypeString = "Special Power";
                        break;
                    case 3:
                        model.SalerOwnTypeString = "General Owner";
                        break;
                    case 4:
                        model.SalerOwnTypeString = "အမွေဆက်ခံ";
                        break;
                    case 5:
                        model.SalerOwnTypeString = "ဂရံဂတုံး";
                        break;
                    case 6:
                        model.SalerOwnTypeString = "Other";
                        break;
                    case 7:
                        model.SalerOwnTypeString = "အဆက်ဆက်စာချုပ်";
                        break;
                    default:
                        model.SalerOwnTypeString = "";
                        break;
                }
               
                switch (model.Ownership)
                {
                    case "Grant_Original":
                        model.Ownership = "Grant (မူရင်း)";
                        break;
                    case "Grant_Copy":
                        model.Ownership = "Grant (မိတ္တူ)";
                        break;
                    case "Permit_Original":
                        model.Ownership = "Permit (မူရင်း)";
                        break;
                    case "Permit_Copy":
                        model.Ownership = "Permit (မိတ္တူ)";
                        break;
                    case "FreeHoldLand":
                        model.Ownership = "FreeHold Land";
                        break;
                    case "BCC":
                        model.Ownership = "BCC";
                        break;
                    case "Contract":
                        model.Ownership = "အစဉ်အဆက်စာချုပ်";
                        break;
                    default:
                        model.Ownership = "";
                        break;
                }


                #endregion

                #region commisssion
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
                #endregion
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
                    .Where(x => x.Code.ToLower() == code.ToLower().Trim()
                    //&& x.Status != "Delete"
                    )
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                if (propertyId >0)
                    return RedirectToAction("PropertyInfo", new { propertyId = propertyId });
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
            if (files == null || files.Count == 0)
            {
                TempData["ErrorMessage"] = "No files were uploaded.";
                return RedirectToAction("Gallery", new { propertyId = propertyId });
            }

            int filecount = 0;

            foreach (var file in files)
            {
                try
                {
                    if (file.Length > 0 && IsImage(file))
                    {
                        //string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{file.FileName}";
                        //var filePath = Path.Combine(_env.WebRootPath, "PropertyPhoto", uniqueFileName);

                        //Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        //using (var stream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(stream);
                        //}

                        var uploadFolder = Path.Combine(_env.WebRootPath, "PropertyPhoto");
                        Directory.CreateDirectory(uploadFolder); // Ensure the folder exists

                        // Generate a unique file name
                        string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Path.GetFileNameWithoutExtension(file.FileName)}.jpg";

                        // Reduce image size and save it asynchronously
                        var filePath = Path.Combine(uploadFolder, uniqueFileName);
                        await ImageHelper.ReduceImageSizeAsync(file, 800, 600, uploadFolder, uniqueFileName);

                        var image = new PhotoModel
                        {
                            PropertyId = propertyId,
                            Title = file.FileName,
                            Location = uniqueFileName,
                            //purposeid = purposeId, // Save PurposeId to the database
                            //TempId = TempId // Save TempId to the database
                        };

                        _context.Photo.Add(image);
                        filecount++;
                    }
                }
                catch (Exception ex)
                {
                    //TempData["ErrorMessage"] = $"An error occurred while processing {file.FileName}: {ex.Message}";
                }
            }
            if(filecount > 0)
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Files uploaded successfully!";
            }
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


        public async Task<IActionResult> AlertCenter()
        {
            var alert = await _context.Alert.AsNoTracking().ToListAsync();

            var code = alert.Select(x => x.Code).ToList();

            var propertyByCode = await _context.Property.AsNoTracking()
                .Select(x => new
                {
                    x.Street,
                    x.Comment,
                    x.Code,
                    x.Id,
                    x.AvailableDate
                })
                .Where(x => code.Contains(x.Code??""))
                .ToListAsync();

            var alertModels = new List<AlertViewModel>();
            foreach(var item in alert)
            {
                var property = propertyByCode.FirstOrDefault(x => x.Code == item.Code);

                alertModels.Add(new AlertViewModel
                {
                    Id = item.Id,
                    PropertyId = property == null? 0 : property.Id,
                    Message = item.Message,
                    Status = item.Status,
                    AvailableDate = property == null? null : property.AvailableDate,
                    Code = item.Code,
                    Address = property == null?  "" : property.Street??"",
                    Info = property == null? "" : property.Comment ?? ""
                });
            }

            return View(alertModels);
        }

        [HttpPost]
        public async Task<IActionResult> AlertStatusUpdate(int updateId)
        {
            var alert = await _context.Alert.FirstOrDefaultAsync(x => x.Id == updateId);
            if(alert != null)
            {
                alert.Status = "Read";
                _context.Alert.Update(alert);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AlertCenter");
        }

        [HttpPost]
        public async Task<IActionResult> AlertDelete(int id)
        {
            var alert = await _context.Alert.FirstOrDefaultAsync(x => x.Id == id);
            if (alert != null)
            {
                _context.Alert.Remove(alert);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AlertCenter");
        }

        public async Task<IActionResult> ExportPropertiesToExcel()
        {
            var properties = await _context.Property
                .Include(x => x.Owner)
                .Include(x => x.PropertyFacilities)
                .Include(x => x.LastCheckedBy)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            var groupedProperties = properties
                .GroupBy(p => new
                {
                    p.PropertyType,
                    p.Purpose
                })
                .ToList();

            var propertyTypes = await _context.PropertyType.AsNoTracking().ToListAsync();
            var buildingTypes = await _context.BuildingType.AsNoTracking().ToListAsync();

            foreach (var group in groupedProperties)
            {
                string sheetName = propertyTypes.Where(x => x.ShortCode == group.Key.PropertyType).Select(x => x.TypeName).FirstOrDefault()?? group.Key.PropertyType;
                //string sheetName = $"{group.Key.PropertyType}";

                if(group.Key.Purpose == "Sale")
                {
                    var saleSheet = package.Workbook.Worksheets.Add($"{sheetName} Sale");

                    AddPropertiesGroup1(saleSheet, group.Where(p => p.Purpose == "Sale").ToList(), propertyTypes, buildingTypes);
                }

                if (group.Key.Purpose == "Rent")
                {
                    var rentSheet = package.Workbook.Worksheets.Add($"{sheetName} Rent");
                    AddPropertiesGroup1(rentSheet, group.Where(p => p.Purpose == "Rent").ToList(), propertyTypes, buildingTypes);
                }
            }

            // Save the Excel file into a MemoryStream
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0; // Reset stream position to the beginning

            // Return the Excel file as a download
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Properties.xlsx");
        }

        private void AddPropertiesGroup1(ExcelWorksheet worksheet, List<PropertyModel> properties, List<PropertyTypeModel> propertyTypes, List<BuildingTypeModel> buildingTypes)
        {
            #region ownership
            Dictionary<int, string> SalerOwnerTypeMapping = new Dictionary<int, string>
            {
                { 0, "" },
                { 1, "အမည်ပေါက်" },
                { 2, "Special Power" },
                { 3, "General Owner" },
                { 4, "အမွေဆက်ခံ" },
                { 5, "ဂရံဂတုံး" },
                { 7, "အဆက်ဆက်စာချုပ်" },
                { 6, "Other" },
            };

            Dictionary<string, string> OwnershipMapping = new Dictionary<string, string>
            {
                { "", "" },
                { "Grant_Original", "Grant (မူရင်း)" },
                { "Grant_Copy", "Grant (မိတ္တူ)" },
                { "Permit_Original", "Permit (မူရင်း)" },
                { "Permit_Copy", "Permit (မိတ္တူ)" },
                { "FreeHoldLand", "FreeHold Land" },
                { "BCC", "BCC" },
                { "Contract", "အစဉ်အဆက်စာချုပ်" },
            };
            #endregion

            #region commission
            Dictionary<int, string> SaleCommissionMapping = new Dictionary<int, string>
            {
                { 0, "" },
                { 1, "2% (အပြည့်အဝ)" },
                { 2, "1% (တစ်ဝက်)" },
                { 3, "1/3 (သုံးပုံတစ်ပုံ)" },
                { 4, "အကျိုးတူ (အညီမျှ)" },
                { 5, "အဆင်ပြေသလို ညှိယူရန်" },
            };

            Dictionary<int, string> RentCommissionMapping = new Dictionary<int, string>
            {
                { 0, "" },
                { 1, "၁လစာ (အိမ်ရှင်+အိမ်ငှား)" },
                { 2, "၁လစာ (တစ်ခြမ်း)" },
                { 3, "1/3 (သုံးပုံတစ်ပုံ)" },
                { 4, "အကျိုးတူ (အညီမျှ)" },
                { 5, "အဆင်ပြေသလို ညှိယူရန်" },
            };
            #endregion

            int colCellNo = 1;

            worksheet.Cells[1, colCellNo++].Value = "Code";
            worksheet.Cells[1, colCellNo++].Value = "OwnerName";
            worksheet.Cells[1, colCellNo++].Value = "OwnerPhone";
            worksheet.Cells[1, colCellNo++].Value = "Address";
            worksheet.Cells[1, colCellNo++].Value = "Ward";
            worksheet.Cells[1, colCellNo++].Value = "Township";
            worksheet.Cells[1, colCellNo++].Value = "Floor";
            if(properties.Count > 0 && properties[0].PropertyType == "H")
                worksheet.Cells[1, colCellNo++].Value = "Building Type";
            worksheet.Cells[1, colCellNo++].Value = "Sqft";

            if(properties.Count > 0 && properties[0].PropertyType != "L" && properties[0].PropertyType != "I")
                worksheet.Cells[1, colCellNo++].Value = "Room";
            worksheet.Cells[1, colCellNo++].Value = "Building";

            if(properties.Count > 0 && properties[0].PropertyType == "C")
                worksheet.Cells[1, colCellNo++].Value = "Condo Name";

            worksheet.Cells[1, colCellNo++].Value = "Price";
            if (properties.Count > 0 && properties[0].PropertyType != "L" && properties[0].PropertyType != "I")
            {
                worksheet.Cells[1, colCellNo++].Value = "N.of M-Bed";
                worksheet.Cells[1, colCellNo++].Value = "N.of S-Bed";
                worksheet.Cells[1, colCellNo++].Value = "Facilities";
            }
            worksheet.Cells[1, colCellNo++].Value = "Comment";
            worksheet.Cells[1, colCellNo++].Value = "%";
            worksheet.Cells[1, colCellNo++].Value = (properties.Count > 0 && properties[0].Purpose == "Sale") ? "SalerOwnType" : "Ownership";
            worksheet.Cells[1, colCellNo++].Value = "Status";
            worksheet.Cells[1, colCellNo++].Value = "Post Status";
            worksheet.Cells[1, colCellNo++].Value = "Registration Date";
            worksheet.Cells[1, colCellNo++].Value = "Available Date";
            worksheet.Cells[1, colCellNo++].Value = "Last Check Date";
            worksheet.Cells[1, colCellNo++].Value = "Last Check By";
            worksheet.Cells[1, colCellNo++].Value = "Remark";
            worksheet.Cells[1, colCellNo++].Value = "Map";

            // Apply styles to headers
            using (var range = worksheet.Cells[1, 1, 1, --colCellNo])
            {
                range.Style.Font.Bold = true;
                //range.Style.Font.Color.SetColor(Color.White);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#A3C9F1"));
                range.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#1D3A6A"));
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
            worksheet.Row(1).Height = 25;
            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells[1, 1, 1, colCellNo].AutoFilter = true;

            // Fill data
            var row = 2;
            foreach (var property in properties)
            {
                int rowCellNo = 1;

                var backgroundColor = row % 2 == 0 ? ColorTranslator.FromHtml("#F4F4F4") : ColorTranslator.FromHtml("#FFFFFF");

                worksheet.Cells[row, rowCellNo++].Value = property.Code;
                worksheet.Cells[row, rowCellNo++].Value = property.Owner != null ? property.Owner.OwnerName : "";
                worksheet.Cells[row, rowCellNo++].Value = property.Owner != null ? property.Owner.OwnerPhone : "";
                worksheet.Cells[row, rowCellNo++].Value = property.Street;
                worksheet.Cells[row, rowCellNo++].Value = property.Ward;
                worksheet.Cells[row, rowCellNo++].Value = property.Township;
                worksheet.Cells[row, rowCellNo++].Value = property.Floor == 0? "(လုံးချင်း/မြေကွက်)" : property.Floor == 1? "Ground Floor" : property.Floor + " Floor";
                if (properties.Count > 0 && properties[0].PropertyType == "H")
                {
                    int buildingTypeId = int.TryParse(property.BuildingType, out int result) ? result : 0;
                    worksheet.Cells[row, rowCellNo++].Value = buildingTypes.Where(x => x.Id == buildingTypeId).Select(x => x.Name).FirstOrDefault() ?? property.BuildingType;
                }

                worksheet.Cells[row, rowCellNo++].Value = property.Size;
                if (property.PropertyType != "L" && property.PropertyType != "I")
                    worksheet.Cells[row, rowCellNo++].Value = property.Room;
                worksheet.Cells[row, rowCellNo++].Value = property.Building;
                if (properties.Count > 0 && properties[0].PropertyType == "C")
                    worksheet.Cells[row, rowCellNo++].Value = property.CondoName;

                worksheet.Cells[row, rowCellNo++].Value = property.Purpose == "Sale" ? property.SalePrice : property.RentPrice;
                worksheet.Cells[row, rowCellNo].Style.Numberformat.Format = "#,##0";
                if (property.PropertyType != "L" && property.PropertyType != "I")
                {
                    worksheet.Cells[row, rowCellNo++].Value = "M-" + property.MasterBed;
                    worksheet.Cells[row, rowCellNo++].Value = "S-" + property.SingleBed;
                    worksheet.Cells[row, rowCellNo++].Value = property.PropertyFacilities != null ? string.Join(", ", property.PropertyFacilities.Select(x => x.Facility).ToList()) : "";
                }
                worksheet.Cells[row, rowCellNo++].Value = property.Comment;
                worksheet.Cells[row, rowCellNo++].Value = property.Purpose == "Sale"? SaleCommissionMapping[property.SaleCommission] : RentCommissionMapping[property.RentCommision];
                worksheet.Cells[row, rowCellNo++].Value = property.Purpose == "Sale" ? SalerOwnerTypeMapping[property.SalerOwnType] : OwnershipMapping[property.Ownership];
                worksheet.Cells[row, rowCellNo++].Value = property.Status;
                worksheet.Cells[row, rowCellNo++].Value = property.PostStatus;
                worksheet.Cells[row, rowCellNo++].Value = property.RegistrationDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, rowCellNo].Style.Numberformat.Format = "MMM dd, yyyy";
                worksheet.Cells[row, rowCellNo++].Value = property.AvailableDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, rowCellNo].Style.Numberformat.Format = "MMM dd, yyyy";
                worksheet.Cells[row, rowCellNo++].Value = property.LastCheckedDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, rowCellNo].Style.Numberformat.Format = "MMM dd, yyyy";
                worksheet.Cells[row, rowCellNo++].Value = property.LastCheckedBy != null ? property.LastCheckedBy.EmployeeName : "";
                worksheet.Cells[row, rowCellNo++].Value = property.Remark;
                worksheet.Cells[row, rowCellNo++].Value = property.Map;

                //worksheet.Cells[row, 15].Style.Numberformat.Format = "$#,##0.00";  // Price formatting

                // Apply border to data rows
                using (var range = worksheet.Cells[row, 1, row, --rowCellNo])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    range.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#4A4A4A"));
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void AddPropertiesToSheet(ExcelWorksheet worksheet, List<PropertyModel> properties, List<PropertyTypeModel> propertyTypes, List<BuildingTypeModel> buildingTypes)
        {
            // Add headers for the sheet
            worksheet.Cells[1, 1].Value = "No.";
            worksheet.Cells[1, 2].Value = "Code";
            worksheet.Cells[1, 3].Value = "Building";
            worksheet.Cells[1, 4].Value = "Room";
            worksheet.Cells[1, 5].Value = "Floor";
            worksheet.Cells[1, 6].Value = "Street";
            worksheet.Cells[1, 7].Value = "Ward";
            worksheet.Cells[1, 8].Value = "CondoName";
            worksheet.Cells[1, 9].Value = "Township";
            worksheet.Cells[1, 10].Value = "Property Type";
            worksheet.Cells[1, 11].Value = "Building Type";
            worksheet.Cells[1, 12].Value = "Purpose";
            worksheet.Cells[1, 13].Value = "Size";
            worksheet.Cells[1, 14].Value = "Area";
            worksheet.Cells[1, 15].Value = "Price";
            worksheet.Cells[1, 16].Value = "Master Bedroom";
            worksheet.Cells[1, 17].Value = "Single Bedroom";
            worksheet.Cells[1, 18].Value = "Comment";
            worksheet.Cells[1, 19].Value = "SaleCommission";
            worksheet.Cells[1, 20].Value = "RentCommision";
            worksheet.Cells[1, 21].Value = "Remark";
            worksheet.Cells[1, 22].Value = "Ownership";
            worksheet.Cells[1, 23].Value = "Registration Date";
            worksheet.Cells[1, 24].Value = "Available Date";
            worksheet.Cells[1, 25].Value = "Last Check Date";
            worksheet.Cells[1, 26].Value = "Last Check By";
            worksheet.Cells[1, 27].Value = "OwnerName";
            worksheet.Cells[1, 28].Value = "OwnerPhone";
            worksheet.Cells[1, 29].Value = "Map";
            worksheet.Cells[1, 30].Value = "Face";
            worksheet.Cells[1, 31].Value = "SalerOwnType";
            worksheet.Cells[1, 32].Value = "Status";
            worksheet.Cells[1, 33].Value = "Post Status";
            worksheet.Cells[1, 34].Value = "Facilities";

            // Apply styles to headers
            using (var range = worksheet.Cells[1, 1, 1, 34])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            // Fill data
            var row = 2;
            int count = 1;
            foreach (var property in properties)
            {
                int buildingTypeId = int.TryParse(property.BuildingType, out int result) ? result : 0;

                worksheet.Cells[row, 1].Value = count;
                worksheet.Cells[row, 2].Value = property.Code;
                worksheet.Cells[row, 3].Value = property.Building;
                worksheet.Cells[row, 4].Value = property.Room;
                worksheet.Cells[row, 5].Value = property.Floor;
                worksheet.Cells[row, 6].Value = property.Street;
                worksheet.Cells[row, 7].Value = property.Ward;
                worksheet.Cells[row, 8].Value = property.CondoName;
                worksheet.Cells[row, 9].Value = property.Township;
                worksheet.Cells[row, 10].Value = propertyTypes.Where(x => x.ShortCode == property.PropertyType).Select(x => x.TypeName).FirstOrDefault() ?? property.PropertyType;
                worksheet.Cells[row, 11].Value = buildingTypes.Where(x => x.Id == buildingTypeId).Select(x => x.Name).FirstOrDefault() ?? property.BuildingType;
                worksheet.Cells[row, 12].Value = property.Purpose;
                worksheet.Cells[row, 13].Value = property.Size;
                worksheet.Cells[row, 14].Value = property.Area;
                worksheet.Cells[row, 15].Value = property.Purpose == "Sale" ? property.SalePrice : property.RentPrice;
                worksheet.Cells[row, 16].Value = property.MasterBed;
                worksheet.Cells[row, 17].Value = property.SingleBed;
                worksheet.Cells[row, 18].Value = property.Comment;
                worksheet.Cells[row, 19].Value = property.SaleCommission;
                worksheet.Cells[row, 20].Value = property.RentCommision;
                worksheet.Cells[row, 21].Value = property.Remark;
                worksheet.Cells[row, 22].Value = property.Ownership;
                worksheet.Cells[row, 23].Value = property.RegistrationDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, 24].Value = property.AvailableDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, 25].Value = property.LastCheckedDate.ToString("MMM dd, yyyy");
                worksheet.Cells[row, 26].Value = property.LastCheckedBy != null ? property.LastCheckedBy.EmployeeName : "";
                worksheet.Cells[row, 27].Value = property.Owner != null ? property.Owner.OwnerName : "";
                worksheet.Cells[row, 28].Value = property.Owner != null ? property.Owner.OwnerPhone : "";
                worksheet.Cells[row, 29].Value = property.Map;
                worksheet.Cells[row, 30].Value = property.Face;
                worksheet.Cells[row, 31].Value = property.SalerOwnType;
                worksheet.Cells[row, 32].Value = property.Status;
                worksheet.Cells[row, 33].Value = property.PostStatus;
                worksheet.Cells[row, 34].Value = property.PropertyFacilities != null ? string.Join(", ", property.PropertyFacilities.Select(x => x.Facility).ToList()) : "";

                // Apply number formatting to specific columns
                worksheet.Cells[row, 15].Style.Numberformat.Format = "#,##0";  // Price formatting
                //worksheet.Cells[row, 15].Style.Numberformat.Format = "$#,##0.00";  // Price formatting
                worksheet.Cells[row, 13].Style.Numberformat.Format = "#,##0";  // Size formatting

                // Apply date formatting
                worksheet.Cells[row, 23].Style.Numberformat.Format = "MMM dd, yyyy";  // Registration Date
                worksheet.Cells[row, 24].Style.Numberformat.Format = "MMM dd, yyyy";  // Available Date
                worksheet.Cells[row, 25].Style.Numberformat.Format = "MMM dd, yyyy";  // Last Check Date

                // Apply border to data rows
                using (var range = worksheet.Cells[row, 1, row, 34])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                row++;
                count++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }


        //private void AddPropertiesToSheet(ExcelWorksheet worksheet, List<PropertyModel> properties, List<PropertyTypeModel> propertyTypes, List<BuildingTypeModel> buildingTypes)
        //{
        //    // Add headers for the sheet
        //    worksheet.Cells[1, 1].Value = "No.";
        //    worksheet.Cells[1, 2].Value = "Code";
        //    worksheet.Cells[1, 3].Value = "Building";
        //    worksheet.Cells[1, 4].Value = "Room";
        //    worksheet.Cells[1, 5].Value = "Floor";
        //    worksheet.Cells[1, 6].Value = "Street";
        //    worksheet.Cells[1, 7].Value = "Ward";
        //    worksheet.Cells[1, 8].Value = "CondoName";
        //    worksheet.Cells[1, 9].Value = "Township";
        //    worksheet.Cells[1, 10].Value = "Property Type";
        //    worksheet.Cells[1, 11].Value = "Building Type";
        //    worksheet.Cells[1, 12].Value = "Purpose";
        //    worksheet.Cells[1, 13].Value = "Size";
        //    worksheet.Cells[1, 14].Value = "Area";
        //    worksheet.Cells[1, 15].Value = "Price";
        //    worksheet.Cells[1, 16].Value = "Master Bedroom";
        //    worksheet.Cells[1, 17].Value = "Single Bedroom";
        //    worksheet.Cells[1, 18].Value = "Comment";
        //    worksheet.Cells[1, 19].Value = "SaleCommission";
        //    worksheet.Cells[1, 20].Value = "RentCommision";
        //    worksheet.Cells[1, 21].Value = "Remark";
        //    worksheet.Cells[1, 22].Value = "Ownership";
        //    worksheet.Cells[1, 23].Value = "Registration Date";
        //    worksheet.Cells[1, 24].Value = "Available Date";
        //    worksheet.Cells[1, 25].Value = "Last Check Date";
        //    worksheet.Cells[1, 26].Value = "Last Check By";
        //    worksheet.Cells[1, 27].Value = "OwnerName";
        //    worksheet.Cells[1, 28].Value = "OwnerPhone";
        //    worksheet.Cells[1, 29].Value = "Map";
        //    worksheet.Cells[1, 30].Value = "Face";
        //    worksheet.Cells[1, 31].Value = "SalerOwnType";
        //    worksheet.Cells[1, 32].Value = "Status";
        //    worksheet.Cells[1, 33].Value = "Post Status";
        //    worksheet.Cells[1, 34].Value = "Facilities";

        //    // Fill data
        //    var row = 2;
        //    int count = 1;
        //    foreach (var property in properties)
        //    {
        //        int buildingTypeId = int.TryParse(property.BuildingType, out int result) ? result : 0;
        //        worksheet.Cells[row, 1].Value = count;
        //        worksheet.Cells[row, 2].Value = property.Code;
        //        worksheet.Cells[row, 3].Value = property.Building;
        //        worksheet.Cells[row, 4].Value = property.Room;
        //        worksheet.Cells[row, 5].Value = property.Floor;
        //        worksheet.Cells[row, 6].Value = property.Street;
        //        worksheet.Cells[row, 7].Value = property.Ward;
        //        worksheet.Cells[row, 8].Value = property.CondoName;
        //        worksheet.Cells[row, 9].Value = property.Township;
        //        worksheet.Cells[row, 10].Value = propertyTypes.Where(x => x.ShortCode == property.PropertyType).Select(x => x.TypeName).FirstOrDefault()?? property.PropertyType;
        //        worksheet.Cells[row, 11].Value = buildingTypes.Where(x => x.Id == buildingTypeId).Select(x => x.Name).FirstOrDefault()?? property.BuildingType;
        //        worksheet.Cells[row, 12].Value = property.Purpose;
        //        worksheet.Cells[row, 13].Value = property.Size;
        //        worksheet.Cells[row, 14].Value = property.Area;
        //        worksheet.Cells[row, 15].Value = property.Purpose == "Sale"? property.SalePrice : property.RentPrice;
        //        worksheet.Cells[row, 16].Value = property.MasterBed;
        //        worksheet.Cells[row, 17].Value = property.SingleBed;
        //        worksheet.Cells[row, 18].Value = property.Comment;
        //        worksheet.Cells[row, 19].Value = property.SaleCommission;
        //        worksheet.Cells[row, 20].Value = property.RentCommision;
        //        worksheet.Cells[row, 21].Value = property.Remark;
        //        worksheet.Cells[row, 22].Value = property.Ownership;
        //        worksheet.Cells[row, 23].Value = property.RegistrationDate.ToString("MMM dd, yyyy");
        //        worksheet.Cells[row, 24].Value = property.AvailableDate.ToString("MMM dd, yyyy");
        //        worksheet.Cells[row, 25].Value = property.LastCheckedDate.ToString("MMM dd, yyyy");
        //        worksheet.Cells[row, 26].Value = property.LastCheckedBy != null? property.LastCheckedBy.EmployeeName : "";
        //        worksheet.Cells[row, 27].Value = property.Owner != null? property.Owner.OwnerName : "";
        //        worksheet.Cells[row, 28].Value = property.Owner != null? property.Owner.OwnerPhone : "";
        //        worksheet.Cells[row, 29].Value = property.Map;
        //        worksheet.Cells[row, 30].Value = property.Face;
        //        worksheet.Cells[row, 31].Value = property.SalerOwnType;
        //        worksheet.Cells[row, 32].Value = property.Status;
        //        worksheet.Cells[row, 33].Value = property.PostStatus;
        //        worksheet.Cells[row, 34].Value = property.PropertyFacilities != null? string.Join(", ", property.PropertyFacilities.Select(x => x.Facility).ToList()) : "";
        //        row++;
        //        count++;
        //    }
        //}

    }
}

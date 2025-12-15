using System.Net.Quic;
using CloseFriendMyanamr.ViewModel.Mobile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobilePropertyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MobilePropertyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("list")]
        public async Task<IActionResult> GetTeachers(PropertyListReq req, CancellationToken cancellationToken)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                var query = await _context.Property.AsNoTracking()
                    .Where(x => x.Status != "Deleted")
                    // .Include(x => x.Photos)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new ProptertyDto
                    {
                        Id = x.Id,
                        Code = x.Code?? "",
                        Status = x.Status?? "N/A",
                        RentPrice = x.RentPrice,
                        SalePrice = x.SalePrice,
                        Remark = x.Remark ?? "",
                        Address = x.Street??"N/A",
                        Comment = x.Comment??"N/A",
                        Township = x.Township,

                        // ImageUrls = x.Photos != null? x.Photos.Select(p => $"{baseUrl}/PropertyPhoto/{p.Location}").ToList():new List<string>(),

                        Phone = "+959782229078",
                        Name = "Close Friend Myanmar",
                        Email = "closefriendmyanmar@gmail.com",
                        Role = "Managing Director",
                        // Facilities = x.PropertyFacilities != null? x.PropertyFacilities.Select(f => f.Facility).ToList() : new List<string>()
                    })
                    .Skip((req.CurrentPageNumber -1) * req.PageSize)
                    .Take(req.PageSize)
                    .ToListAsync(cancellationToken);

                var pageInfoDto = new PageInfoDto
                {
                    PageNumber = req.CurrentPageNumber,
                    PageSize = req.PageSize
                };

                if (query.Any())
                {
                    int totalItems = await _context.Property.AsNoTracking()
                        .Where(x => x.Status != "Deleted")
                        .CountAsync();

                    int LastPageNumber = (int)Math.Ceiling((double)totalItems / req.PageSize);

                    pageInfoDto.LastPageNumber = LastPageNumber;
                    pageInfoDto.TotalItemCount = totalItems;
                }

                // query = query.OrderBy(x => x.Id).ToList();

                var data = new PropertyListRes
                {
                    PageInfo = pageInfoDto,
                    Propterties = query
                };

                var result = new ApiResponse<PropertyListRes>
                {
                    Success = true,
                    Message = "Property retrieved successfully",
                    Data = data
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResult = new ApiResponse<NothingDto>
                {
                    Success = false,
                    Message = $"Error retrieving properties: {ex.Message}",
                    Data = null
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
            }
        }

    }
}

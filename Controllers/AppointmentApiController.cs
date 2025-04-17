using CloseFriendMyanamr.Models.Appointment;
using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Mvc;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Book")]
        public async Task<IActionResult> Book([FromBody] AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add validation for date and time parsing
            if (!DateTime.TryParse(appointmentDto.Date, out var date))
            {
                return BadRequest("Invalid date format");
            }

            if (!TimeSpan.TryParse(appointmentDto.Time, out var time))
            {
                return BadRequest("Invalid time format");
            }

            var appointment = new AppointmentDomain
            {
                Name = appointmentDto.Name,
                Email = appointmentDto.Email,
                Phone = appointmentDto.Phone,
                Date = date,
                Time = time,
                Message = appointmentDto.Message
            };

            _context.BookAppointment.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
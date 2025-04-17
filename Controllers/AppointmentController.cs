using CloseFriendMyanamr.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleDataWebsite.Data;

namespace CloseFriendMyanamr.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointment
        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            var appointments = from a in _context.BookAppointment
                               select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                appointments = appointments.Where(a =>
                    a.Name.Contains(searchString) ||
                    a.Email.Contains(searchString) ||
                    a.Phone.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                appointments = appointments.Where(a => a.Status == statusFilter);
            }

            var viewModel = new AppointmentListViewModel
            {
                Appointments = await appointments.OrderByDescending(a => a.Date).ToListAsync(),
                CurrentFilter = searchString,
                StatusFilter = statusFilter
            };

            return View(viewModel);
        }

        // Add your existing Book action here
        // ...

        // New action to update status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.BookAppointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

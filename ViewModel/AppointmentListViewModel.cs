using CloseFriendMyanamr.Models.Appointment;

namespace CloseFriendMyanamr.ViewModel
{
    public class AppointmentListViewModel
    {
        public IEnumerable<AppointmentDomain> Appointments { get; set; }
        public string CurrentFilter { get; set; }
        public string StatusFilter { get; set; }
    }
}

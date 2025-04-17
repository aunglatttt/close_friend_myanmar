using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models.Appointment
{
    public class AppointmentDomain
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        public string Message { get; set; }
        public string Status { get; set; } = "New";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CloseFriendMyanamr.Models.UserManagement
{
    public class AgentModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Agent Name is required.")]
        [DisplayName("Agent Name")]
        public string AgentName { get; set; }


        [Required(ErrorMessage = "Agent Phone is required.")]
        //[Phone(ErrorMessage = "Please enter a valid phone number.")]
        [DisplayName("Agent Phone Number")]
        public string AgentPhone { get; set; }


        //[Required(ErrorMessage = "Address is required.")]
        [DisplayName("Address")]
        public string? Address { get; set; }

        public string? FBPage { get; set; }

        public string? Remark { get; set; }

        public int ShowProperty { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

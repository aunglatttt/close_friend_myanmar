using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CloseFriendMyanamr.Models.ClientManagement;

namespace CloseFriendMyanamr.Models.UserManagement
{
    public class ClientModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Client Name is required.")]
        [DisplayName("Client Name")]
        public string ClientName { get; set; }


        [Required(ErrorMessage = "Client Phone is required.")]
        //[Phone(ErrorMessage = "Please enter a valid phone number.")]
        [DisplayName("Client Phone Number")]
        public string ClientPhone { get; set; }


        //[Required(ErrorMessage = "Address is required.")]
        [DisplayName("Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "RegistrationDate is required.")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        //[Required(ErrorMessage = "ShownProperty is required.")]
        [DisplayName("Shown Property")]
        public int ShownProperty { get; set; }


        public string? Remark { get; set; }

        public virtual List<ClientRequirementModel>? ClientRequirements { get; set; }

    }
}

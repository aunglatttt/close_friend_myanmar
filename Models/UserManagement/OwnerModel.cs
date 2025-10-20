using CloseFriendMyanamr.Models.CompanyInformation;
using CloseFriendMyanamr.Models.Property;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.UserManagement
{
    public class OwnerModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Owner Name")]
        public string OwnerName { get; set; }


        //[Required(ErrorMessage = "Owner Phone is required.")]
        //[Phone(ErrorMessage = "Please enter a valid phone number.")]
        [DisplayName("Owner Phone Number")]
        public string? OwnerPhone { get; set; }

        //[Required(ErrorMessage = "Type is required.")]
        public string? Type { get; set; }

        //[Required(ErrorMessage = "Address is required.")]
        [DisplayName("Address")]
        public string? Address { get; set; }


        //[Required(ErrorMessage = "Remark is required.")]
        public string? Remark { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual List<PropertyModel>? Properties { get; set; }


        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }

    }
}

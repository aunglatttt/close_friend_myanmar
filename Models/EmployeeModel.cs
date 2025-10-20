using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models
{
    public class EmployeeModel : BaseDomain
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        public string EmployeeName { get; set; }


        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Login Name is required.")]
        [DisplayName("Login Name")]
        public string LoginName { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [NotMapped]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "EmployeeType is required.")]
        [DisplayName("Employee Type")]
        public int EmployeeTypeId { get; set; }

        public bool Status { get; set; }
        public virtual EmployeeType? EmployeeType { get; set; }


        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }
    }
}

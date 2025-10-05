using CloseFriendMyanamr.Models.UserManagement;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models.ClientManagement
{
    public class ClientRequirementModel : BaseDomain

    {
        [Key]
        public int Id { get; set; }

        public int ClientId { get; set; }

        [Required(ErrorMessage = "Purpose is required.")]
        [StringLength(50)]
        public string Purpose { get; set; }

        //[Required(ErrorMessage = "Property Type is required.")]
        [DisplayName("Property Type")]
        public string? PropertyType { get; set; }

        [DisplayName("Building Condition")]
        public string? BuildingCondition { get; set; }

        //[Required(ErrorMessage = "Building Type is required.")]
        [DisplayName("Building Type")]
        public string? BuildingType { get; set; }


        [DisplayName("လမ်း")]
        public string? Street { get; set; }

        [DisplayName("ရပ်ကွက်")]
        public string? Ward { get; set; }

        [DisplayName("အိမ်ယာ/ကွန်ဒို (၁ ခုထပ်ပိုပါက \",\" ခြားပြီး ရိုက်ပေးပါ)")]
        public string? CondoName { get; set; }

        [DisplayName("Price From (သိန်း)")]
        public int StartPrice { get; set; } = 0;

        [DisplayName("Price To (သိန်း)")]
        public int EndPrice { get; set; } = 0;

        [DisplayName("Area (Min)")]
        public int Area { get; set; } = 0;

        [DisplayName("MB (Min)")]
        public int MasterBed { get; set; } = 0;

        [DisplayName("Floor (Min))")]
        public int FloorMin { get; set; } = 0;

        [DisplayName("Floor (Max)")]
        public int FloorMax { get; set; } = 0;

        [DisplayName("အခြား (Special Request!)")]
        public string? SpecialRequest { get; set; }

        public string? Township { get; set; }
        public string? Facilities { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "New";
        public DateTime? RequestDate { get; set; }
        public virtual ClientModel? Client { get; set; }
    }
}

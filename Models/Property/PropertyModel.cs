using CloseFriendMyanamr.Models.UserManagement;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.Property
{
    public class PropertyModel
    {
        [Key]
        public int Id { get; set; }

        public string? Code { get; set; }

        [Required(ErrorMessage = "Owner is required.")]
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Property Type is required.")]
        [DisplayName("Property Type")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Building Type is required.")]
        [DisplayName("Building Type")]
        public string BuildingType { get; set; }
        public string? Building { get; set; }
        public string? Purpose { get; set; }
        [NotMapped]
        public bool PurposeSale { get; set; }
        [NotMapped]
        public bool PurposeRent { get; set; }
        public string? Room { get; set; }
        public int Floor { get; set; } = 0;

        [Required(ErrorMessage = "Street is required.")]
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? CondoName { get; set; }

        [Required(ErrorMessage = "Township is required.")]
        public string Township { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime AvailableDate { get; set; } = DateTime.Now;
        public DateTime LastCheckedDate { get; set; } = DateTime.Now;

        public string? Remark { get; set; }
        public string? Map { get; set; }
        public string? Face { get; set; }
        public string Ownership { get; set; }
        public int SalerOwnType { get; set; }
        public string? Size { get; set; }
        public int? Area { get; set; }
        public int MasterBed { get; set; } = 0;
        public int SingleBed { get; set; } = 0;
        public string? Comment { get; set; }
        public int SalePrice { get; set; } = 0;
        public int RentPrice { get; set; } = 0;
        public int SaleCommission { get; set; } = 0;
        [NotMapped]
        public string? SaleCommissionString { get; set; }
        public int RentCommision { get; set; } = 0;
        [NotMapped]
        public string? RentCommisionString { get; set; }
        //public string? Facilities { get; set; } // not found at old db
        public int? LastCheckedById { get; set; }
        public string? Status { get; set; }
        public string? PostStatus { get; set; }
        public string? SalePriceCurrency { get; set; }
        public string? RentPriceCurrency { get; set; }

        public virtual OwnerModel? Owner { get; set; }

        [NotMapped]
        public string? OwnerName { get; set; }
        [NotMapped]
        public string? OwnerPhone { get; set; }
        [NotMapped]
        public string? OwnerAddress { get; set; }
        [NotMapped]
        public string OwnerTypeSelect { get; set; }
        public virtual List<PhotoModel>? Photos { get; set; }
        public virtual List<PropertyFacilityModel>? PropertyFacilities { get; set; }
        public virtual EmployeeModel? LastCheckedBy { get; set; }
    }
}

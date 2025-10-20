using CloseFriendMyanamr.Models.CompanyInformation;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloseFriendMyanamr.Models.Property
{
    public class PhotoModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string? Title { get; set; }
        public string Location { get; set; }
        public virtual PropertyModel Property { get; set; }


        [ForeignKey("CompanyInfo")]
        public int CPI { get; set; }
        public virtual CompanyInfoModel? CompanyInfo { get; set; }

    }
}

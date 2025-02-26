namespace CloseFriendMyanamr.Models.Property
{
    public class PropertyFacilityModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Facility { get; set; }
        public virtual PropertyModel Property { get; set; }
    }
}

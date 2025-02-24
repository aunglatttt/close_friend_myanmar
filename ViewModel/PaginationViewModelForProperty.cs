namespace CloseFriendMyanamr.ViewModel
{
    public class PaginationViewModelForProperty
    {
        public List<PropertyViewModel> Propertys { get; set; } = new List<PropertyViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public List<int> PageSizes { get; set; } = new List<int> { 5, 10, 50, 100 };

        // Filter parameters
        public string? PropertyType { get; set; }
        public string? BuildingType { get; set; }
        public string? OwnerAgent { get; set; }
        public string? Purpose { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public string? CondoName { get; set; }
        public string? Face { get; set; }
        public string? BuildingNo { get; set; }
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? CommentInfo { get; set; }
        public string? Ownership { get; set; }
        public string? SalerOwnType { get; set; }
        public int? Floor { get; set; }
        public string? Size { get; set; }
        public string? Area { get; set; }
        public int? MasterRoom { get; set; }
        public int? SigleRoom { get; set; }
        public string? Status { get; set; }
        public string? PostStatus { get; set; }
        public List<string> Townships { get; set; }
        public List<string> Facilities { get; set; }
    }
}

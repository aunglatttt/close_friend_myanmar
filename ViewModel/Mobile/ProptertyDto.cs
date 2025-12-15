namespace CloseFriendMyanamr.ViewModel.Mobile;

public record class ProptertyDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Code { get; set; }
    public string Township { get; set; }
    public string? Room { get; set; }
    public decimal RentPrice { get; set; }
    public string Street { get; set; }
    public string Comment { get; set; }
    public decimal? SalePrice { get; set; }
    public string Remark { get; set; }
    public string Address { get; set; }

    public string Name { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }

    public List<string> ImageUrls { get; set; }
    public List<string> Facilities { get; set; }
}

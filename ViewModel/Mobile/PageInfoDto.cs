namespace CloseFriendMyanamr.ViewModel.Mobile;

public record class PageInfoDto
{
public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int LastPageNumber { get; set; }
    public int TotalItemCount { get; set; }
}

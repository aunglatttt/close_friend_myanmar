namespace CloseFriendMyanamr.ViewModel.Mobile;

public record class PropertyListReq
{
    public string SearchText { get; set; }
    public int CurrentPageNumber { get; set; }
    public int PageSize { get; set; }
}

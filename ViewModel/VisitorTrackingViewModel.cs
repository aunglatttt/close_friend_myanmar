namespace CloseFriendMyanamr.ViewModel
{
    public class VisitorTrackingViewModel
    {
        public string PageUrl { get; set; }
        public string Referrer { get; set; }
        public string UserAgent { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public string Language { get; set; }
        public string Timezone { get; set; }
        public string Timestamp { get; set; }
    }
}

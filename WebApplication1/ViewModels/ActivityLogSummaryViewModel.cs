namespace WebApplication1.ViewModels
{
    public sealed class ActivityLogSummaryViewModel
    {
        public string OperationType { get; set; }
            = string.Empty;
        public string? BlogPostId { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; }
        = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}

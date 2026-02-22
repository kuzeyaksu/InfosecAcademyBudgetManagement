namespace InfosecAcademyBudgetManagement.Models
{
    public class NotificationViewModel
    {
        public List<NotificationItem> Items { get; set; } = [];
        public int CriticalCount => Items.Count(x => x.Level == NotificationLevel.Critical);
        public int WarningCount => Items.Count(x => x.Level == NotificationLevel.Warning);
        public int InfoCount => Items.Count(x => x.Level == NotificationLevel.Info);
    }

    public class NotificationItem
    {
        public NotificationLevel Level { get; set; } = NotificationLevel.Info;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public enum NotificationLevel
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }
}

namespace InfosecAcademyBudgetManagement.Models
{
    public class AuditLogListItemViewModel
    {
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? IpAddress { get; set; }
    }
}

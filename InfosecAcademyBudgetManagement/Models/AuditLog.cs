using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class AuditLog
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(200)]
        public string? UserName { get; set; }

        [StringLength(10)]
        public string Method { get; set; } = string.Empty;

        [StringLength(400)]
        public string Path { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? Detail { get; set; }
    }
}

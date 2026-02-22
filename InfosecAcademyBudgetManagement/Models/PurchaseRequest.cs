using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class PurchaseRequest : Base
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Talep Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Range(0.01, 999999999)]
        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [StringLength(30)]
        [Display(Name = "Durum")]
        public string Status { get; set; } = "Beklemede";

        [Display(Name = "Talep Eden")]
        public string? RequestedByUserId { get; set; }

        [Display(Name = "Onaylayan")]
        public string? ApprovedByUserId { get; set; }

        [Display(Name = "Onay/Zaman")]
        public DateTime? ApprovedAt { get; set; }

        [StringLength(500)]
        [Display(Name = "Red Gerekçesi")]
        public string? RejectionReason { get; set; }
    }
}

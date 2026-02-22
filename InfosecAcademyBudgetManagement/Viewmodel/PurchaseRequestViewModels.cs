using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class PurchaseRequestListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
    }

    public class PurchaseRequestFormViewModel
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
    }

    public class PurchaseRequestDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}

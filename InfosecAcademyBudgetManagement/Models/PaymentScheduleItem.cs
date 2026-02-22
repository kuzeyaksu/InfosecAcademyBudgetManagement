using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class PaymentScheduleItem : Base
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Tür")]
        public string Type { get; set; } = "Borç"; // Borç / Alacak

        [Range(0.01, 999999999)]
        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Vade Tarihi")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Ödendi/Tahsil Edildi")]
        public bool IsSettled { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "İşlem Tarihi")]
        public DateTime? SettledAt { get; set; }

        [Display(Name = "Cari")]
        public int? CounterpartyId { get; set; }
        public Counterparty? Counterparty { get; set; }

        [StringLength(500)]
        [Display(Name = "Not")]
        public string? Note { get; set; }
    }
}

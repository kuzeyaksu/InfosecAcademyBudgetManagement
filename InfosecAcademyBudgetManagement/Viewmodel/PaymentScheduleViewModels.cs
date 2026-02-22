using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class PaymentScheduleListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsSettled { get; set; }
        public string CounterpartyName { get; set; } = "-";
    }

    public class PaymentScheduleFormViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Tür")]
        public string Type { get; set; } = "Borç";

        [Range(0.01, 999999999)]
        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Vade Tarihi")]
        public DateTime DueDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Cari")]
        public int? CounterpartyId { get; set; }

        [StringLength(500)]
        [Display(Name = "Not")]
        public string? Note { get; set; }
    }
}

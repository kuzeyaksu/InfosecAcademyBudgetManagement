using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class IncomeItem : Base
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Gelir Adı")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 999999999)]
        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Kaynak")]
        public string Source { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Müşteri / Ödeyen")]
        public string? Counterparty { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class Counterparty : Base
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Ad")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Tür")]
        public string Type { get; set; } = "Müşteri";

        [StringLength(30)]
        [Display(Name = "Vergi / TCKN")]
        public string? TaxNumber { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(120)]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [StringLength(250)]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [StringLength(500)]
        [Display(Name = "Not")]
        public string? Note { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class Category : Base
    {
        [Required]
        [Display(Name = "Kategori")]
        public string? Name { get; set; }

        public ICollection<Cost.CostItem> Costs { get; set; } = [];
    }
}


using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class BudgetPlan : Base
    {
        [Range(2000, 2100)]
        [Display(Name = "Yıl")]
        public int Year { get; set; }

        public ICollection<BudgetLine> Lines { get; set; } = [];
    }
}


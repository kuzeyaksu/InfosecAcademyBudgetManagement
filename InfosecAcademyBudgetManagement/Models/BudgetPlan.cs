using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class BudgetPlan : Base
    {
        [Range(2000, 2100)]
        [Display(Name = "Yıl")]
        public int Year { get; set; }

        [Range(1, 99)]
        [Display(Name = "Versiyon")]
        public int VersionNo { get; set; } = 1;

        public ICollection<BudgetLine> Lines { get; set; } = [];
    }
}


using System.ComponentModel.DataAnnotations;

namespace InfosecAcademyBudgetManagement.Models
{
    public class BudgetLine : Base
    {
        public int BudgetPlanId { get; set; }
        public BudgetPlan? BudgetPlan { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        public decimal PlannedAmount { get; set; }
    }
}


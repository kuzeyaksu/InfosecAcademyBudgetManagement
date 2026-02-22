namespace InfosecAcademyBudgetManagement.Models
{
    public class AdvancedReportViewModel
    {
        public int Year { get; set; }
        public List<string> MonthLabels { get; set; } = [];
        public List<decimal> IncomeMonthly { get; set; } = [];
        public List<decimal> ExpenseMonthly { get; set; } = [];
        public List<decimal> NetMonthly { get; set; } = [];
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Net => TotalIncome - TotalExpense;
    }
}

using System.ComponentModel.DataAnnotations;
using InfosecAcademyBudgetManagement.Models;

namespace Cost
{
    public class CostItem:Base
    {
       
        [Display(Name = "Ad")]
        public string? Name { get; set; }

        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentContentType { get; set; }
        public long? DocumentSize { get; set; }
    }



    
}


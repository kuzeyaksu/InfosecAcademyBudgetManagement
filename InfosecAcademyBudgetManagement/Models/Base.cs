using System;
using System.Security.AccessControl;

namespace InfosecAcademyBudgetManagement.Models;

public class Base
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? Updatedby { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
    
}


namespace COMP2139_assign01.Models;

public class ApplicationUserCategory
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
        
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
}
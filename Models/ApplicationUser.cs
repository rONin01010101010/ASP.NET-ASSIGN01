using Microsoft.AspNetCore.Identity;

namespace COMP2139_assign01.Models;


public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string Address { get; set; }
    public byte[] profilePicture { get; set; }
    public string PhoneNumber { get; set; }
    
    public ICollection<ApplicationUserCategory> UserCategories { get; set; }

    
}
using System.ComponentModel.DataAnnotations;

namespace COMP2139_assign01.Areas.User.Models;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
        
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }
        
    [Display(Name = "Address")]
    public string Address { get; set; }
        
    [Display(Name = "City")]
    public string City { get; set; }
        
    [Display(Name = "State")]
    public string State { get; set; }
        
    [Display(Name = "Zip Code")]
    public string ZipCode { get; set; }
        
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
}
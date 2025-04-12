using System.ComponentModel.DataAnnotations;

namespace COMP2139_assign01.Areas.User.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
        
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
        
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
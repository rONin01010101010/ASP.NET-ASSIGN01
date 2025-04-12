using System.ComponentModel.DataAnnotations;

namespace COMP2139_assign01.Areas.User.Models;

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
        
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string Password { get; set; }
        
    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
        
    // Hidden field to store the reset token
    public string Code { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace COMP2139_assign01.Areas.User.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}
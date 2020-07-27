using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MMOServer.ViewModels.Account
{
  public class LoginViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    // External Auth
    public string ReturnUrl { get; set; }
  
    public IList<AuthenticationScheme> ExternalLogins { get; set; }
  
  }
}

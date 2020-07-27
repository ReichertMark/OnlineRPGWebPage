using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.ViewModels.Account
{
  public class LoginAndCreateSessionViewModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class LoginAndCreateSessionResponseViewModel
  {
    public string UserId { get; set; }
    public string ErrorMessage { get; set; }
  }
}

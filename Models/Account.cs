using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.Models
{
  public class Account : IdentityUser
  {
    public string City { get; set; }

    public ICollection<Character> Characters { get; set; }
  }
}

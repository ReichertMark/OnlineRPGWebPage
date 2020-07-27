using Microsoft.AspNetCore.Http;
using MMOServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.ViewModels
{

  public class CharacterCreateViewModel
  {
    public string UserId { get; set; }

    [Required]
    [MaxLength(20, ErrorMessage = "Name cannot exceed 20 Characters")]
    public string Name { get; set; }
    public int AccountSlot { get; set; }
    public int Gender { get; set; }
    public int Race { get; set; }
    public int Class { get; set; }
    public int Realm { get; set; }
  }

  public class CharacterCreateResponseViewModel
  {
    public string ErrorMessage { get; set; }
  }
}

using System.Collections.Generic;
using MMOServer.Models;

namespace MMOServer.ViewModels
{
  public class CharacterGetAllResonseViewModel
  {
    public string ErrorMessage { get; set; }
    public IEnumerable<Character> Characters { get; set; }
  }
}

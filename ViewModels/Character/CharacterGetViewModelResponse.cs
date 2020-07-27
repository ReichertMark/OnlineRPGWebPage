using MMOServer.Models;

namespace MMOServer.ViewModels
{
  public class CharacterGetViewModelResponse
  {
    public string ErrorMessage { get; set; }
    public Character Character { get; set; }
  }
}

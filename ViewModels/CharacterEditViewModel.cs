using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.ViewModels
{
  public class CharacterEditViewModel : CharacterCreateViewModel
  {
    public long Id { get; set; }
    public string ExistingImagePath { get; set; }
  }
}

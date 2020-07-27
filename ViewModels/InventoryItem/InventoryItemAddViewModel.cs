using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.ViewModels
{
  public class InventoryItemAddViewModel
  {
    public string CharacterName { get; set; }
    public string TemplateId { get; set; }
    public int SlotPosition { get; set; }
    public int StackSize { get; set; }

  }
}

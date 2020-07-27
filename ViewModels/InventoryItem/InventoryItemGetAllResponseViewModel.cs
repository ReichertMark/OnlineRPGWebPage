using MMOServer.Models;
using System.Collections.Generic;


namespace MMOServer.ViewModels
{
  public class InventoryItemGetAllResponseViewModel
  {
    public string ErrorMessage { get; set; }
    public IEnumerable<InventoryItem> InventoryItems { get; set; }
  }
}

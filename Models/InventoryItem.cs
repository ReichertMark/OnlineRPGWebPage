using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MMOServer.Models
{
  public class InventoryItem
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long InventoryItemId { get; set; }

    // This Template id is the Name of the item in Unreal Engine.
    public string TemplateId { get; set; }

    // Position in Inventory
    public int SlotPosition { get; set; }

    // Number of items
    public int StackSize { get; set; }

    // Character linkage
    [JsonIgnore] 
    [Required]
    public long CurrentOwnerId { get; set; }
    [JsonIgnore] 
    [Required]
    public Character Character { get; set; }



  }
}

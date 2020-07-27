using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMOServer.Models
{

  public class Character
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CharacterId { get; set; }

    // Common Character Data
    public string Name { get; set; }
    public int AccountSlot { get; set; }
    public int Gender { get; set; }
    public int Race { get; set; }
    public int Class { get; set; }
    public int Realm { get; set; }
    public int Region { get; set; }

    // Appearance
    public int Model { get; set; }


    // Experience and Stuff
    public int Level { get; set; }
    public long Experience { get; set; }
    public int RealmLevel { get; set; }
    public long RealmPoints { get; set; }
    public long BountyPoints { get; set; }

    // Currency
    public int Copper { get; set; }
    public int Silver { get; set; }
    public int Gold { get; set; }
    public int Platinum { get; set; }


    // Stats
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Quickness { get; set; }
    public int Intelligence { get; set; }
    public int Piety { get; set; }
    public int Empathy { get; set; }
    public int Charisma { get; set; }

    // Transform
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }

    // Bind Data
    public float BindPosX { get; set; }
    public float BindPosY { get; set; }
    public float BindPosZ { get; set; }
    public float BindRotX { get; set; }
    public float BindRotY { get; set; }
    public float BindRotZ { get; set; }
    public int BindRegion { get; set; }



    // TODO: Skills and Abilities 

    // Inventory
    public ICollection<InventoryItem> InventoryItems { get; set; }


    // Account linkage
    [Required]
    public string CurrentAccountId { get; set; }

    [Required]
    public Account Account { get; set; }
  }


}

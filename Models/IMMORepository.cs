using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.Models
{
  // What operations are supported ? (not how)
  public interface IMMORepository
  {
    Character GetCharacter(long id);
    IEnumerable<Character> GetAllCharacters();
    Character Add(Character character);
    Character Update(Character character);
    Character Delete(long id);


    Task AddItem(InventoryItem item);
    IEnumerable<InventoryItem> GetAllInventoryItems();
    InventoryItem DeleteItem(long id);
  }
  

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.Models
{
  public class SqlMMORepository : IMMORepository
  {
    private readonly MMODbContext context;
    public SqlMMORepository(MMODbContext context)
    {
      this.context = context;
    }

    public Character Add(Character character)
    {
      context.Characters.Add(character);
      context.SaveChanges();
      return character;
    }

    public Character Delete(long id)
    {
      Character character = context.Characters.Find(id);

      if (character != null)
      {
        context.Characters.Remove(character);
        context.SaveChanges();
      }

      return character;
    }

    public IEnumerable<Character> GetAllCharacters()
    {
      return context.Characters;
    }

    public Character GetCharacter(long id)
    {
      return context.Characters.Find(id);
    }

    public Character Update(Character characterChanges)
    {
      var item = context.Characters.Attach(characterChanges);

      item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
      context.SaveChanges();

      return characterChanges;
    }


    // --- Inventory ---


    public Task AddItem(InventoryItem item)
    {
      Task one = context.Items.AddAsync(item).AsTask();
      Task two = context.SaveChangesAsync();
      return Task.WhenAll(one, two);
    }

    public IEnumerable<InventoryItem> GetAllInventoryItems()
    {
      return context.Items;
    }

    public InventoryItem DeleteItem(long id)
    {
      var item = context.Items.Find(id);

      if (item != null)
      {
        context.Items.Remove(item);
        context.SaveChanges();
      }

      return item;

    }

  }
}

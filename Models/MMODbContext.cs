using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MMOServer.Models
{
  public class MMODbContext : IdentityDbContext<Account>
  {
    public MMODbContext(DbContextOptions<MMODbContext> options)
      : base(options)
    {

    }

    //Include Sets for each entity in the model Used to query and save instances
    public DbSet<Character> Characters { get; set; }
    public DbSet<InventoryItem> Items { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    // https://www.youtube.com/watch?v=qDUS8ocavBU&list=PL6n9fhu94yhVkdrusLaQsfERmL_Jh4XmU&index=51
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // https://www.entityframeworktutorial.net/efcore/configure-one-to-many-relationship-using-fluent-api-in-ef-core.aspx
      modelBuilder.Entity<Character>()
      .HasOne<Account>(s => s.Account)
      .WithMany(g => g.Characters)
      .HasForeignKey(s => s.CurrentAccountId)
      .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<InventoryItem>()
      .HasOne<Character>(s => s.Character)
      .WithMany(g => g.InventoryItems)
      .HasForeignKey(s => s.CurrentOwnerId)
      .OnDelete(DeleteBehavior.Cascade);

      //foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      //{
      //  foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
      //}
    }
  }
}

namespace COMP2139_assign01.Data;
using COMP2139_assign01.Models; 
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }
    
    public DbSet<Inventory> Inventory { get; set; } 
    
    public DbSet<InventoryTask> Tasks { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Inventory>().HasData(
            new Inventory { InventoryId = 1, Name = "Carpet", Quantity = 5, category = "Household", Price = 50 },
            new Inventory { InventoryId = 2, Name = "Laptop", Quantity = 7, category = "Electronics", Price = 500 }
        );

        
        modelBuilder.Entity<InventoryTask>()
            .HasOne(it => it.Inventory) 
            .WithMany(i => i.Tasks)     
            .HasForeignKey(it => it.InventoryId);  
    }
}
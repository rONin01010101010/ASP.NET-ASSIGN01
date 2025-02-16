namespace COMP2139_assign01.Models;
using System.ComponentModel.DataAnnotations;

public class Inventory
{
    public Inventory()
    {
        Tasks = new List<InventoryTask>();
        Name = string.Empty;
        category = string.Empty;
    }
    
    public int InventoryId { get; set; }
    
    [Required]
    [Display(Name = "Product Name")]
    public string Name { get; set; }
    
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string category { get; set; }
    public int TotalStock { get; set; }

    // Navigation properties
    public ICollection<InventoryTask> Tasks { get; set; }  

    // Remove the self-referencing relationship as we're moving to the new Product model
    // public List<Inventory> LowStockItems { get; set; }
}
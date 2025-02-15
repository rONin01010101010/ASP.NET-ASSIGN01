namespace COMP2139_assign01.Models;
using System.ComponentModel.DataAnnotations;

public class Inventory
{

    public Inventory()
    {
            Tasks = new List<InventoryTask>();
            LowStockItems = new List<Inventory>();
            Name = string.Empty;
            category = string.Empty;
    }
    
    
    
    public List<Inventory> LowStockItems { get; set; }
    
    //public string Category { get; set; } = string.Empty;
    
    public int TotalStock { get; set; }
    public int InventoryId { get; set; }
    public int Quantity { get; set; }
    
    [Required]
    [Display(Name = "Product Name")]
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public string category { get; set; }
    
    public ICollection<InventoryTask> Tasks { get; set; }  

}
namespace COMP2139_assign01.Models;
using System.ComponentModel.DataAnnotations;
public class InventoryTask
{
    [Key]
    public int InventoryTaskId { get; set; }
    
    public int Quantity { get; set; }
    
    [Required] 
    public decimal Price { get; set; }
    
    public Inventory? Inventory { get; set; }

    public string category { get; set; }
    
    
    public int InventoryId { get; set; }
    
    public InventoryTask()
    {
        category = string.Empty; // Initialize to avoid the non-nullable warning
    }
    
}
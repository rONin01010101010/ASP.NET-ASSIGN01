using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP2139_assign01.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Guest Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string GuestEmail { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? GuestPhone { get; set; }

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [NotMapped]
        public DateTime LocalOrderDate
        {
            get => OrderDate.ToLocalTime();
            set => OrderDate = value.ToUniversalTime();
        }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
